using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Net;

namespace dermai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Kernel _kernel;

        public HomeController(ILogger<HomeController> logger, Kernel kernel)
        {
            _logger = logger;
            _kernel = kernel;
        }

        private int ObtenerIdUsuarioActual()
        {
            string email = HttpContext.Session.GetString("usu");
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }
            return BD.ObtenerIdUsuarioPorEmail(email);
        }

        private IActionResult VerificarSesion()
        {
            string email = HttpContext.Session.GetString("usu");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }
            return null; // Sesión válida
        }

        private IActionResult VerificarUsuario(int idUsuario)
        {
            if (idUsuario == 0)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Login", "Account");
            }
            return null; // Usuario válido
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult InicioA()
        {
            string email = HttpContext.Session.GetString("usu");
            if (!string.IsNullOrEmpty(email))
            {
                Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
                if (usuario != null && usuario.IdPerfil > 0)
                {
                    Perfil perfil = BD.ObtenerPerfilPorId(usuario.IdPerfil);
                    if (perfil != null && !string.IsNullOrEmpty(perfil.CaracteristicasPiel))
                    {
                        var caracteristicas = perfil.CaracteristicasPiel?
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => c.Trim())
                            .ToList() ?? new List<string>();

                        if (caracteristicas.Any())
                        {
                            TempData["Mensaje2"] = caracteristicas.First();
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            return View("Inicio");
        }

        [HttpPost]
        public async Task<IActionResult> GenerarRutina(int IdPerfil)
        {
            return await GenerarRutinaInternal(IdPerfil);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarRutina()
        {
            int idUsuario = ObtenerIdUsuarioActual();
            var redirect = VerificarSesion();
            if (redirect != null)
            {
                return redirect;
            }

            redirect = VerificarUsuario(idUsuario);
            if (redirect != null)
            {
                return redirect;
            }

            Usuario usuario = BD.ObtenerUsuarioPorEmail(HttpContext.Session.GetString("usu"));
            if (usuario == null || usuario.IdPerfil == 0)
            {
                TempData["Error"] = "Primero debes completar tu perfil.";
                return RedirectToAction("ModificarRutina", "Home");
            }

            return await GenerarRutinaInternal(usuario.IdPerfil);
        }

        private async Task<IActionResult> GenerarRutinaInternal(int idPerfil)
        {
            int idUsuario = ObtenerIdUsuarioActual();
            var redirect = VerificarSesion();
            if (redirect != null)
            {
                return redirect;
            }

            redirect = VerificarUsuario(idUsuario);
            if (redirect != null)
            {
                return redirect;
            }

            Perfil perfil = BD.ObtenerPerfilPorId(idPerfil);
            if (perfil == null)
            {
                return NotFound("Perfil no encontrado.");
            }

            string prompt = CrearPrompt(perfil);

            try
            {
                var chatService = _kernel.GetRequiredService<IChatCompletionService>();
                var respuesta = await LlamarAPIConReintentos(() => ObtenerRespuestaIA(chatService, prompt));

                Rutina rutina = new Rutina
                (
                    Rutinas: respuesta,
                    RutinaFinal: respuesta,
                    IdUsuario: idUsuario
                );

                BD.GuardarRutina(rutina);

                TempData["RutinaGenerada"] = respuesta;
                return RedirectToAction("VerRutinaGuardada", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar la rutina de IA o al guardar en BD.");
                TempData["Error"] = "Error al generar la rutina (falla de IA/servidor). Intenta de nuevo. Detalles: " + ex.Message;

                // Redirige al formulario para que el usuario pueda intentarlo de nuevo.
                return RedirectToAction("ModificarRutina", "Home");
            }
        }
        private async Task<string> ObtenerRespuestaIA(IChatCompletionService chatService, string prompt)
        {
            var history = new ChatHistory();
            history.AddSystemMessage("Eres un dermatólogo experto en cuidado de la piel.");
            history.AddUserMessage(prompt);

            var promptSettings = new GeminiPromptExecutionSettings
            {
                Temperature = 0.7,
                TopP = 0.95
            };

            // Intentar obtener la respuesta con reintentos
            return await LlamarAPIConReintentos(async () => 
            {
                var resultado = await chatService.GetChatMessageContentAsync(history, promptSettings);
                return resultado.Content; // Usar el Content de la respuesta
            });
        }

        private async Task<T> LlamarAPIConReintentos<T>(Func<Task<T>> llamadaApi)
        {
            int intentos = 0;
            const int maxIntentos = 5;
            int tiempoEspera = 2000;

            while (intentos < maxIntentos)
            {
                try
                {
                    return await llamadaApi();
                }
                // 👉 Cambia la captura para ser más tolerante o simplemente usa un catch general para probar
                catch (HttpRequestException ex) // Captura el HttpRequestException
                {
                    // Verifica explícitamente el código de estado antes de reintentar
                    if (ex.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        intentos++;
                        _logger.LogWarning($"Error 429 recibido. Reintentando ({intentos}/{maxIntentos}). Tiempo de espera: {tiempoEspera}ms");
                        
                        await Task.Delay(tiempoEspera);
                        tiempoEspera *= 2; // Exponential backoff
                    }
                    else
                    {
                        // Re-lanza si no es un 429, ya que no queremos reintentar otros errores
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Puedes añadir una captura general para registrar otros errores
                    _logger.LogError(ex, "Error desconocido en LlamarAPIConReintentos.");
                    throw;
                }
            }

            throw new Exception($"No se pudo completar la solicitud a la API tras {maxIntentos} intentos. La API está sobrecargada.");
        }

        private string CrearPrompt(Perfil perfil)
        {
            string caracteristicasTxt = FormatearLista(perfil.CaracteristicasPiel);
            string preferenciasTxt = FormatearLista(perfil.PreferenciaProducto);

            return $@"Eres un dermatólogo experto. Con los siguientes datos del usuario, crea una rutina de cuidado de la piel personalizada:
            🧴 Características: {caracteristicasTxt}
            💄 Preferencias: {preferenciasTxt}
            💰 Presupuesto: {perfil.Presupuesto}
            ⏰ Frecuencia: {perfil.FrecuenciaRutina}

            Devuelve una rutina dividida en pasos de mañana y noche, con recomendaciones de tipos de productos (no marcas).";
        }

        private string FormatearLista(string texto)
        {
            return string.Join(", ", 
                texto?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(c => c.Trim()) ?? new List<string>());
        }

        public IActionResult VerRutinaGuardada()
        {
            string rutinaTexto = TempData["RutinaGenerada"]?.ToString();
            if (!string.IsNullOrEmpty(rutinaTexto))
            {
                ViewBag.Rutina = rutinaTexto;
                return View("MostrarRutina");
            }

            int idUsuario = ObtenerIdUsuarioActual();
            var redirect = VerificarSesion();
            if (redirect != null)
            {
                return redirect;
            }

            Rutina rutina = BD.ObtenerRutinaPorUsuario(idUsuario);

            if (rutina == null)
            {
                ViewBag.Mensaje = "No se encontró una rutina guardada. ¡Crea la tuya!";
            }
            else
            {
                ViewBag.Rutina = rutina.RutinaFinal;
            }

            return View("MostrarRutina");
        }

        public IActionResult ModificarRutina()
        {
            return View("HacerRutina");
        }

        public IActionResult IrTipoPiel()
        {
            return View("InfoTipoDePiel");
        }

        public IActionResult IrRecomendaciones()
        {
            return View("Recomendacion");
        }
    }
}
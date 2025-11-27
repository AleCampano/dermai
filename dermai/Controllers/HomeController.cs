using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using System.Text;
using System.Text.Json;

namespace dermai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey = "AIzaSyDvyu3mslsgWteDVXGf5ZSO3-U-df5QMME";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        private IActionResult RedirectIfNoSession()
        {
            string email = HttpContext.Session.GetString("usu");
            if (email == null)
                return RedirectToAction("Login", "Account");

            return null;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult InicioA()
{
    IActionResult redirect = RedirectIfNoSession();
    if (redirect != null) return redirect;

    string email = HttpContext.Session.GetString("usu");
    Usuario usuario = BD.ObtenerUsuarioPorEmail(email);

    string tipoPiel = "No definido";

    // Leer desde sesión primero
    var tipoPielSession = HttpContext.Session.GetString("tipoPiel");
    if (!string.IsNullOrEmpty(tipoPielSession))
    {
        tipoPiel = tipoPielSession;
    }
    else
    {
        if (usuario != null && usuario.IdPerfil > 0)
        {
            tipoPiel = BD.ObtenerTipoPielPorUsuario(email);
            // guardo en sesión para próximas visitas
            HttpContext.Session.SetString("tipoPiel", tipoPiel);
        }
    }

    // opcional: loguear para depuración
    _logger.LogInformation("InicioA - tipoPiel session: {tpSession}, tipoPiel final: {tp}", tipoPielSession, tipoPiel);

    ViewBag.TipoPiel = tipoPiel;
    return View("Inicio");
}


        [HttpGet]
        public async Task<IActionResult> GenerarRutina()
        {
            IActionResult redirect = RedirectIfNoSession();
            if (redirect != null) return redirect;

            string email = HttpContext.Session.GetString("usu");
            Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
            if (usuario == null || usuario.IdPerfil == 0)
            {
                TempData["Error"] = "Primero completa tu perfil.";
                return RedirectToAction("ModificarRutina");
            }

            return await GenerarRutinaInternal(usuario.IdPerfil);
        }

        [HttpPost]
        public Task<IActionResult> GenerarRutina(int IdPerfil)
        {
            return GenerarRutinaInternal(IdPerfil);
        }

        [HttpPost]
        [Route("/api/Home/GenerarRutina")]
        public async Task<IActionResult> GenerarRutinaApi()
        {
            try
            {
                IActionResult redirect = RedirectIfNoSession();
                if (redirect != null) return Unauthorized(new { error = "Sesión expirada" });

                string email = HttpContext.Session.GetString("usu");
                Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
                
                if (usuario == null || usuario.IdPerfil == 0)
                    return BadRequest(new { error = "Primero completa tu perfil." });

                Perfil perfil = BD.ObtenerPerfilPorId(usuario.IdPerfil);
                if (perfil == null) 
                    return NotFound(new { error = "Perfil no encontrado." });

                string prompt = CrearPrompt(perfil);
                string respuesta = await LlamarIA(prompt);

                // Guardar rutina en base de datos
                Rutina r = new Rutina(respuesta, respuesta, BD.ObtenerIdUsuarioPorEmail(email));
                BD.GuardarRutina(r);

                return Ok(new { 
                    rutina = respuesta,
                    mensaje = "Rutina generada exitosamente" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GenerarRutinaApi");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<IActionResult> GenerarRutinaInternal(int idPerfil)
        {
            IActionResult redirect = RedirectIfNoSession();
            if (redirect != null) return redirect;

            Perfil perfil = BD.ObtenerPerfilPorId(idPerfil);
            if (perfil == null) return NotFound("Perfil no encontrado.");

            string prompt = CrearPrompt(perfil);

            try
            {
                string respuesta = await LlamarIA(prompt);

                string email = HttpContext.Session.GetString("usu");
                Rutina r = new Rutina(respuesta, respuesta, BD.ObtenerIdUsuarioPorEmail(email));
                BD.GuardarRutina(r);

                TempData["RutinaGenerada"] = respuesta;
                return RedirectToAction("VerRutinaGuardada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error IA");
                TempData["Error"] = "No se pudo generar la rutina. Intenta de nuevo.";
                return RedirectToAction("ModificarRutina");
            }
        }

        private async Task<string> LlamarIA(string prompt)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 1000,
                        topP = 0.8,
                        topK = 40
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash-001:generateContent?key={_geminiApiKey}";
                
                var response = await _httpClient.PostAsync(url, content, cts.Token);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error de API: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (jsonResponse.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var candidate = candidates[0];
                    if (candidate.TryGetProperty("content", out var contentObj) &&
                        contentObj.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var part = parts[0];
                        if (part.TryGetProperty("text", out var text))
                        {
                            return text.GetString() ?? throw new Exception("Respuesta vacía de la IA");
                        }
                    }
                }

                throw new Exception("No se pudo extraer la respuesta de la IA");
            }
            catch (OperationCanceledException)
            {
                throw new Exception("Timeout: La IA tardó demasiado en responder");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error llamando a Gemini. Prompt: {Prompt}", prompt);
                throw new Exception($"Error al comunicarse con la IA: {ex.Message}");
            }
        }

        public IActionResult VerRutinaGuardada()
        {
            string gen = "";
            if (TempData["RutinaGenerada"] != null)
                gen = TempData["RutinaGenerada"].ToString();

            string email = HttpContext.Session.GetString("usu");
            int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
            Rutina r = BD.ObtenerRutinaPorUsuario(idUsuario);

            if (gen != "" || r != null)
            {
                ViewBag.Rutina = gen != "" ? gen : r.RutinaFinal;
                ViewBag.TieneRutina = true;
            }
            else
            {
                ViewBag.Rutina = "No tienes rutina guardada.";
                ViewBag.TieneRutina = false;
            }

            return View("MostrarRutina");
        }

        public IActionResult ModificarRutina()
        {
            return View("HacerRutina");
        }
        

        public IActionResult IrTipoPiel()
{
    IActionResult redirect = RedirectIfNoSession();
    if (redirect != null) return redirect;

    string tipoPiel = ViewBag.TipoPiel as string;  // Usamos ViewBag.TipoPiel que ya está disponible
    if (string.IsNullOrEmpty(tipoPiel))
    {
        tipoPiel = "No definido";  // Valor por defecto en caso de que no esté configurado
    }

    // Si quieres asegurar que sea de la base de datos, puedes obtenerlo de aquí, pero no es necesario
    // string email = HttpContext.Session.GetString("usu");
    // tipoPiel = BD.ObtenerTipoPielPorUsuario(email);

    ViewBag.TipoPiel = tipoPiel;  // Reasignamos el valor a ViewBag.TipoPiel

    return View("InfoTipoDePiel");
}

        public async Task<IActionResult> IrRecomendaciones()
        {
            IActionResult redirect = RedirectIfNoSession();
            if (redirect != null) return redirect;

            string email = HttpContext.Session.GetString("usu");
            int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
            
            Rutina rutinaActual = BD.ObtenerRutinaPorUsuario(idUsuario);
            
            if (rutinaActual == null)
            {
                ViewBag.TieneRutinaPersonalizada = false;
                ViewBag.MensajeSinRutina = "Aún no tienes una rutina personalizada. Para obtener recomendaciones específicas para tu tipo de piel, primero crea tu rutina en la sección 'Crear Rutina'.";
                return View("Recomendacion");
            }

            try
            {
                Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
                Perfil perfil = BD.ObtenerPerfilPorId(usuario.IdPerfil);
                
                string promptRecomendaciones = CrearPromptRecomendaciones(rutinaActual, perfil);
                string recomendaciones = await LlamarIA(promptRecomendaciones);

                ViewBag.RecomendacionesPersonalizadas = recomendaciones;
                ViewBag.TieneRutinaPersonalizada = true;
                ViewBag.MensajeConRutina = "Estas recomendaciones están personalizadas según tu rutina actual y tipo de piel.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando recomendaciones");
                ViewBag.TieneRutinaPersonalizada = false;
                ViewBag.MensajeError = "No se pudieron generar recomendaciones personalizadas en este momento. Te mostramos nuestras recomendaciones generales.";
            }

            return View("Recomendacion");
        }
        
         public IActionResult HacerRutina()
        {
            IActionResult redirect = RedirectIfNoSession();
            if (redirect != null) return redirect;

             return View("HacerRutina");
         }

        private string CrearPromptRecomendaciones(Rutina rutina, Perfil perfil)
        {
            return @$"Eres un dermatólogo experto. Basándote en la siguiente rutina de cuidado facial y las características del usuario, genera recomendaciones de productos específicos:

            RUTINA ACTUAL DEL USUARIO:
            {rutina.RutinaFinal}

            CARACTERÍSTICAS DE PIEL: {perfil.CaracteristicasPiel}
            PREFERENCIAS DE PRODUCTOS: {perfil.PreferenciaProducto}
            PRESUPUESTO: {perfil.Presupuesto}

            INSTRUCCIONES PARA LAS RECOMENDACIONES:
            1. Recomienda productos específicos que complementen la rutina actual
            2. Organiza por categorías: Limpiadores, Tónicos/Esencias, Hidratantes, Sérums, Protectores solares
            3. Incluye 2-3 opciones por categoría considerando el presupuesto: {perfil.Presupuesto}
            4. Prioriza productos que se ajusten a: {perfil.PreferenciaProducto}
            5. Incluye tanto marcas reconocidas como opciones genéricas
            6. Especifica brevemente por qué cada producto es adecuado
            7. Formato claro con listas y puntos

            RESPONDE SOLO CON LAS RECOMENDACIONES, sin introducciones ni conclusiones adicionales.";
        }

        private string CrearPrompt(Perfil p)
        {
            return @$"Eres un dermatólogo experto. Genera una rutina de cuidado facial personalizada con las siguientes características:

            CARACTERÍSTICAS DE PIEL: {p.CaracteristicasPiel}
            PREFERENCIAS DE PRODUCTOS: {p.PreferenciaProducto}
            PRESUPUESTO: {p.Presupuesto}
            FRECUENCIA: {p.FrecuenciaRutina}

            INSTRUCCIONES:
            1. Divide la rutina en MAÑANA y NOCHE
            2. Incluye productos específicos según el presupuesto
            3. Considera las preferencias del usuario
            4. No recomiendes marcas específicas, solo tipos de productos
            5. Sé conciso pero completo
            6. Formato claro con puntos

            RESPONDE SOLO CON LA RUTINA, sin introducciones ni conclusiones.";
        }
    }
}
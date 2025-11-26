using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace dermai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IChatCompletionService _chat;

        public HomeController(ILogger<HomeController> logger, IChatCompletionService chat)
        {
            _logger = logger;
            _chat = chat;
        }

        private string GetEmail()
        {
            return HttpContext.Session.GetString("usu");
        }

        private IActionResult RedirectIfNoSession()
        {
            string email = GetEmail();
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

            string email = GetEmail();
            Usuario usuario = BD.ObtenerUsuarioPorEmail(email);

            if (usuario != null && usuario.IdPerfil > 0)
            {
                Perfil perfil = BD.ObtenerPerfilPorId(usuario.IdPerfil);
                if (perfil != null && perfil.CaracteristicasPiel != null && perfil.CaracteristicasPiel != "")
                {
                    string[] arr = perfil.CaracteristicasPiel.Split(',');
                    if (arr.Length > 0) TempData["Mensaje2"] = arr[0].Trim();
                }
            }

            return View("Inicio");
        }

        [HttpGet]
        public async Task<IActionResult> GenerarRutina()
        {
            IActionResult redirect = RedirectIfNoSession();
            if (redirect != null) return redirect;

            Usuario usuario = BD.ObtenerUsuarioPorEmail(GetEmail());
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

                Rutina r = new Rutina(respuesta, respuesta, BD.ObtenerIdUsuarioPorEmail(GetEmail()));
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
            var history = new ChatHistory();
            history.AddSystemMessage("Eres un dermatólogo experto.");
            history.AddUserMessage(prompt);

            var settings = new GeminiPromptExecutionSettings
            {
                Temperature = 0.7f
            };

            string respuesta = await _chat.GetChatMessageContentAsync(history, settings);

            if (respuesta == null || respuesta == "")
                throw new Exception("Gemini no devolvió contenido.");

            return respuesta;
        }

        public IActionResult VerRutinaGuardada()
        {
            string gen = "";
            if (TempData["RutinaGenerada"] != null)
                gen = TempData["RutinaGenerada"].ToString();

            if (gen != "")
            {
                ViewBag.Rutina = gen;
                return View("MostrarRutina");
            }

            int idUsuario = BD.ObtenerIdUsuarioPorEmail(GetEmail());
            Rutina r = BD.ObtenerRutinaPorUsuario(idUsuario);

            if (r != null)
                ViewBag.Rutina = r.RutinaFinal;
            else
                ViewBag.Rutina = "No tienes rutina guardada.";

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

        private string CrearPrompt(Perfil p)
        {
            return "Genera una rutina de piel con: " +
                   "\nCaracterísticas: " + p.CaracteristicasPiel +
                   "\nPreferencias: " + p.PreferenciaProducto +
                   "\nPresupuesto: " + p.Presupuesto +
                   "\nFrecuencia: " + p.FrecuenciaRutina +
                   "\nDivide en mañana y noche (sin marcas).";
        }
    }
}
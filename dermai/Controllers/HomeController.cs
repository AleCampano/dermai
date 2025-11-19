using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace dermai.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Kernel _kernel;

    public HomeController(ILogger<HomeController> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
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
        }
        return View("Inicio");
    }

    [HttpPost]
    public async Task<IActionResult> GenerarRutina(int IdPerfil)
    {
        return await GenerarRutinaInternal(IdPerfil);
    }

    // Método GET alternativo por si se accede directamente
    public async Task<IActionResult> GenerarRutina()
    {
        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
        if (usuario == null || usuario.IdPerfil == 0)
        {
            TempData["Error"] = "Primero debes completar tu perfil.";
            return RedirectToAction("CompletarFormularioRutina", "User");
        }

        return await GenerarRutinaInternal(usuario.IdPerfil);
    }

    // Método interno para reutilizar la lógica
    private async Task<IActionResult> GenerarRutinaInternal(int idPerfil)
    {
        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
        if (idUsuario == 0)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction("Login", "Account");
        }

        Perfil perfil = BD.ObtenerPerfilPorId(idPerfil);
        if (perfil == null)
        {
            return NotFound("Perfil no encontrado.");
        }

        var caracteristicasList = perfil.CaracteristicasPiel?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .ToList() ?? new List<string>();

        var preferenciasList = perfil.PreferenciaProducto?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .ToList() ?? new List<string>();

        string caracteristicasTxt = string.Join(", ", caracteristicasList);
        string preferenciasTxt = string.Join(", ", preferenciasList);

        string prompt = $@"Eres un dermatólogo experto. Con los siguientes datos del usuario, crea una rutina de cuidado de la piel personalizada:
        🧴 Características: {caracteristicasTxt}
        💄 Preferencias: {preferenciasTxt}
        💰 Presupuesto: {perfil.Presupuesto}
        ⏰ Frecuencia: {perfil.FrecuenciaRutina}

        Devuelve una rutina dividida en pasos de mañana y noche, con recomendaciones de tipos de productos (no marcas).";

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();
        history.AddSystemMessage("Eres un dermatólogo experto en cuidado de la piel.");
        history.AddUserMessage(prompt);

        var promptSettings = new GeminiPromptExecutionSettings
        {
            Temperature = 0.7,
            TopP = 0.95
        };

        var respuesta = await chatService.GetChatMessageContentAsync(history, promptSettings);

        Rutina rutina = new Rutina
        (
            Rutinas: respuesta.Content,
            RutinaFinal: respuesta.Content, 
            IdUsuario: idUsuario
        );

        BD.GuardarRutina(rutina);

        TempData["RutinaGenerada"] = respuesta.Content;
        return RedirectToAction("MostrarRutina", "Home");
    }

    public IActionResult MostrarRutina()
    {
        string rutinaTexto = TempData["RutinaGenerada"]?.ToString() ?? "";
        if (string.IsNullOrEmpty(rutinaTexto))
        {
            ViewBag.Error = "No se pudo recuperar la rutina generada.";
            return RedirectToAction("CompletarFormularioRutina", "User");
        }

        ViewBag.Rutina = rutinaTexto;
        return View();
    }
    public IActionResult VerRutina()
    {
        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }
        
        int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
        
        if (idUsuario == 0)
        {
            ViewBag.Mensaje = "No se encontró el perfil del usuario.";
            return RedirectToAction("CompletarFormularioRutina", "User");
        }
        
        Rutina rutina = BD.ObtenerRutinaPorUsuario(idUsuario);
    
        if (rutina == null)
        {
            ViewBag.Mensaje = "No se encontró una rutina guardada.";
            return View("MostrarRutina");
        }

        ViewBag.Rutina = rutina.RutinaFinal;
        return View("MostrarRutina");
    }

    public IActionResult ModificarRutina()
    {
        return RedirectToAction("CompletarFormularioRutina", "User");
    }

    public IActionResult IrTipoPiel()
    {
        return View ("InfoTipoDePiel");
    }

    public IActionResult IrRecomendaciones()
    {
        return View ("Recomendacion");
    }

    public IActionResult VerificarRutina()
    {
        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
        if (usuario == null || usuario.IdPerfil == 0)
        {
            TempData["Error"] = "Primero debes completar tu perfil.";
            return RedirectToAction("CompletarFormularioRutina", "User");
        }

        int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
        return RedirectToAction("GenerarRutina", "Home", new { IdPerfil = usuario.IdPerfil });
    }
}

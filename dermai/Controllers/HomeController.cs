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
        return View("Inicio");
    }

    [HttpPost]
    public async Task<IActionResult> GenerarRutina(int idPerfil)
    {
        Perfil perfil = BD.ObtenerPerfilPorId(idPerfil);
        if (perfil == null)
        {return NotFound("Perfil no encontrado.");}

        var caracteristicasList = Objeto.StringToList<string>(perfil.CaracteristicasPiel);
        var preferenciasList = Objeto.StringToList<string>(perfil.PreferenciaProducto);

        string caracteristicasTxt = string.Join(", ", caracteristicasList ?? new List<string>());
        string preferenciasTxt = string.Join(", ", preferenciasList ?? new List<string>());

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


        ViewBag.Rutina = respuesta.Content;
        return RedirectToAction("GuardarRutina", "Home");
    }

    public IActionResult GuardarRutina()
    {
        string rutinaTexto = "";
        if (TempData["RutinaGenerada"] != null)
        {
            rutinaTexto = TempData["RutinaGenerada"].ToString();
        }
        else if (TempData["RutinaGenerada"] != null)
        {
            rutinaTexto = TempData["RutinaGenerada"].ToString();        

        }        
        if (string.IsNullOrEmpty(rutinaTexto))
        {
            ViewBag.Error = "No se pudo recuperar la rutina generada.";
            return View("MostrarRutina");
        }

        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
        if (usuario == null)
        {
            ViewBag.Error = "Usuario no encontrado.";
            return View("MostrarRutina");
        }

        Rutina rutina = new Rutina(true, rutinaTexto, usuario.IdUsuario);
        BD.GuardarRutina(rutina);

        TempData["RutinaGenerada"] = respuesta.Content;
        return View("MostrarRutina");
    }

    public IActionResult VerRutina()
    {
        string email = HttpContext.Session.GetString("usu");
        Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
        Rutina rutina = BD.ObtenerRutinaPorUsuario(usuario.IdUsuario);

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

    [HttpPost]
    public IActionResult GuardarRutinaModificada(List<string> caracteristicas, List<string> preferencias, string presupuesto, string frecuencia)
    {
        if ((caracteristicas == null || caracteristicas.Count == 0) ||
        (preferencias == null || preferencias.Count == 0) ||
        string.IsNullOrEmpty(presupuesto) ||
        string.IsNullOrEmpty(frecuencia))
        {
            ViewBag.Error = "Por favor completá todos los campos.";
            return View("HacerRutina");
        }

        string email = HttpContext.Session.GetString("usu");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        Usuario usuario = BD.ObtenerUsuarioPorEmail(email);
        if (usuario == null)
        {
            ViewBag.Error = "Usuario no encontrado.";
            return RedirectToAction("Login", "Account");
        }

        Perfil perfil = new Perfil(Objeto.ListToString(caracteristicas), Objeto.ListToString(preferencias), presupuesto, frecuencia);
        BD.ModificarPerfil(usuario.IdPerfil, perfil);
        return View("Inicio");
    }
}

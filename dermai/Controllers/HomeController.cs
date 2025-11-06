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
        return View("MostrarRutina");
    }

    public IActionResult VerRutina()
    {
        return View("MostrarRutina");
    }

    public IActionResult ModificarRutina()
    {
        return RedirectToAction("CompletarFormularioRutina", "Account");
    }

    public IActionResult GuardarTareaModificada()
    {
        return View("Inicio");
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace dermai.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IKernel _kernel;

    public HomeController(ILogger<HomeController> logger, IKernel kernel)
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
        return View();
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

        var chat = _kernel.GetRequiredService<IChatCompletionService>();
        var chatSession = chat.CreateNewChat();
        var respuesta = await chatSession.SendMessageAsync(prompt);

        ViewBag.Rutina = respuesta.Content;
        return View("MostrarRutina");
    }

    public IActionResult GuardarRutina()
    {
        return View();
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

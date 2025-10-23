using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    { 
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Inicio()
    {
        return View();
    }

    public IActionResult CrearRutina()
    {
        return View();
    }

    public IActionResult GuardarRutina()
    {
        return View(Inicio);
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

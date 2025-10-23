using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    public IActionResult CompletarFormularioPiel()
    {
        return View("IngresoPiel");
    }

    public IActionResult GuardarFormularioPiel()
    {
        return View();
    }

    public IActionResult CompletarFormularioRutina()
    {
        return View("HacerRutina");
    }

    public IActionResult GuardarFormularioRutina()
    {
        return View();
    }
}
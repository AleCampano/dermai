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

    [HttpPost]
    public IActionResult GuardarFormularioPiel(int NivelGrasaPiel, string AlergiaProductos, string IrritacionFrecuencia, string AparicionGranos)
    {
        string tipoPiel;
        if (NivelGrasaPiel < 33)
        {
            tipoPiel = "Piel seca";
        }
        else if (NivelGrasaPiel < 66)
        {
            tipoPiel = "Piel mixta";
        }
        else
        {
            tipoPiel = "Piel grasa";
        }

        string detalles = $"Tipo: {tipoPiel}. " + $"Alergia a productos: {AlergiaProductos}. " + $"Irritación: {IrritacionFrecuencia}. " + $"Granos: {AparicionGranos}.";

        var perfil = new Perfil(detalles, "", "", "");
        int idPerfil = BD.CrearPerfil(perfil);

        TempData["Mensaje"] = "¡Datos de tu piel guardados correctamente!";
        return RedirectToAction("InicioA", "Home");
    }

    public IActionResult CompletarFormularioRutina()
    {
        return View("HacerRutina");
    }

    [HttpPost]
    public IActionResult GuardarFormularioRutina(string[] caracteristicas, string[] preferencias, string presupuesto, string frecuencia)
    {
        string caracteristicasStr = Objeto.ListToString(caracteristicas.ToList());
        string preferenciasStr = Objeto.ListToString(preferencias.ToList());

        var perfil = new Perfil(caracteristicasStr, preferenciasStr, presupuesto, frecuencia);

        int idPerfil = BD.CrearPerfil(perfil);

        TempData["Mensaje"] = "¡Tu rutina fue guardada correctamente!";
        return RedirectToAction("GenerarRutina", "Home", new { idPerfil = idPerfil });
    }
}
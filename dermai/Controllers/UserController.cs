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
    [ValidateAntiForgeryToken]
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

        string email = HttpContext.Session.GetString("usu");
        var usu = BD.ObtenerUsuarioPorEmail(email);

        if (usu == null)
        {
            TempData["Error"] = "Usuario no encontrado";
            return RedirectToAction("Login", "Account");
        }

        var perfil = new Perfil(detalles, "", "", "");

        if (usu.IdPerfil > 0)
        {
            BD.ModificarPerfil(usu.IdPerfil, perfil);
        }
        else
        {
            int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
            int idPerfil = BD.CrearPerfil(idUsuario, perfil);
            BD.AsignarPerfilAUsuario(usu.Email, idPerfil);
        }

        TempData["Mensaje"] = "¡Datos de tu piel guardados correctamente!";
        TempData["Mensaje2"] = tipoPiel;
        return RedirectToAction("InicioA", "Home");
    }

    public IActionResult CompletarFormularioRutina()
    {
        return View("HacerRutina");
    }

    [HttpPost]
    public IActionResult GuardarFormularioRutina(string[] Caracteristicas, string[] Preferencias, string Presupuesto, string Frecuencia)
    {
        string email = HttpContext.Session.GetString("usu");
        var usu = BD.ObtenerUsuarioPorEmail(email);
    
        if (usu == null)
        {
            TempData["Error"] = "Usuario no encontrado";
            return RedirectToAction("Login", "Account");
        }
        
        var caracteristicasList = Caracteristicas?.ToList() ?? new List<string>();
        var preferenciasList = Preferencias?.ToList() ?? new List<string>();
        
        string caracteristicasStr = Objeto.ListToString(caracteristicasList);
        string preferenciasStr = Objeto.ListToString(preferenciasList);

        var perfil = new Perfil(caracteristicasStr, preferenciasStr, Presupuesto, Frecuencia);

        int idPerfil = 0;
        
        if (usu.IdPerfil > 0)
        {
            BD.ModificarPerfil(usu.IdPerfil, perfil);
            idPerfil = usu.IdPerfil;
        }
        else
        {
            int idUsuario = BD.ObtenerIdUsuarioPorEmail(email);
            idPerfil = BD.CrearPerfil(idUsuario, perfil);
            BD.AsignarPerfilAUsuario(usu.Email, idPerfil);
        }

        TempData["Mensaje"] = "¡Tu rutina fue guardada correctamente!";
        return RedirectToAction("GenerarRutina", "Home", new { IdPerfil = idPerfil });
    }

    public IActionResult IrInicio()
    {
        return RedirectToAction("InicioA", "Home");
    }
}
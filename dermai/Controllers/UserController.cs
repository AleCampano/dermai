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

            // 🔹 Construir los detalles del perfil
            string detalles = $"Tipo: {tipoPiel}. " +
                              $"Alergia a productos: {AlergiaProductos}. " +
                              $"Irritación: {IrritacionFrecuencia}. " +
                              $"Granos: {AparicionGranos}.";


            // 🔹 Crear el perfil
            var perfil = new Perfil(
                detalles,
                "", // PreferenciaProducto
                "", // Presupuesto
                ""  // FrecuenciaRutina
            );

            // 🔹 Guardar en la BD a través del modelo BD
            int idPerfil = BD.CrearPerfil(perfil);

            TempData["Mensaje"] = "¡Datos de tu piel guardados correctamente!";

            // 🔹 Redirigir al siguiente formulario
            return RedirectToAction("CompletarFormularioRutina");
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
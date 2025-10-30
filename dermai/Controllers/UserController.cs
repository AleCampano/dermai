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

    public IActionResult GuardarFormularioPiel(int NivelGrasaPiel, string AlergiaProductos, string IrritacionFrecuencia, string AparicionGranos)
    {
        string tipoPiel = NivelGrasaPiel < 33 ? "Piel seca" :
        NivelGrasaPiel < 66 ? "Piel mixta" :
        "Piel grasa";
        
        
        return View("Inicio", "Home");
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


/*[HttpPost]
public IActionResult GuardarFormularioPiel(int NivelGrasaPiel, string AlergiaProductos, string IrritacionFrecuencia, string AparicionGranos)
{
    // 🔹 Determinar tipo general de piel
    string tipoPiel = NivelGrasaPiel < 33 ? "Piel seca" :
                      NivelGrasaPiel < 66 ? "Piel mixta" :
                      "Piel grasa";

    // 🔹 Agregar detalles
    string detalles = $"Tipo: {tipoPiel}. " +
                      $"Alergia a productos: {AlergiaProductos}. " +
                      $"Irritación: {IrritacionFrecuencia}. " +
                      $"Granos: {AparicionGranos}.";

    // 🔹 Obtener ID del usuario actual (ajustar según tu login real)
    int idUsuario = 1; // valor de ejemplo

    // 🔹 Crear el perfil con solo Características de piel (los demás valores vacíos)
    var perfil = new Perfil(
        idUsuario,
        detalles,
        "", // PreferenciaProducto
        "", // Presupuesto
        ""  // FrecuenciaRutina
    );

    // 🔹 Guardar en la base de datos
    string connectionString = "Server=.;Database=dermaiDB;Trusted_Connection=True;TrustServerCertificate=True;";
    using (var db = new SqlConnection(connectionString))
    {
        string sql = @"INSERT INTO Perfiles (IdUsuario, CaracteristicasPiel, PreferenciaProducto, Presupuesto, FrecuenciaRutina)
                       VALUES (@IdUsuario, @CaracteristicasPiel, @PreferenciaProducto, @Presupuesto, @FrecuenciaRutina)";
        db.Execute(sql, perfil);
    }

    // 🔹 Mensaje y redirección
    TempData["Mensaje"] = "¡Datos de tu piel guardados correctamente!";
    return RedirectToAction("CompletarFormularioRutina");
}*/
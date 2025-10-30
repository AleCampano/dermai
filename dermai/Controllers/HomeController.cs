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

    public IActionResult InicioA()
    {
        return View();
    }

    public IActionResult CrearRutina()
    {
        return View("HacerRutina");
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

    public IActionResult guardarInfoUser(List<string> caracteristicas, List<string> preferencias, string presupuesto,string frecuencia)
    {
        // Recuperar usuario logueado (suponiendo que lo guardaste en sesión)
    Usuario user = Objeto.StringToObject<Usuario>(HttpContext.Session.GetString("usuario"));
    if (user == null)
    {
        return RedirectToAction("Login", "Home");
    }

    // Crear objeto InfoUsuario
    InfoUsuario info = new InfoUsuario
    {
        IdUsuario = user.IdPerfil,
        Caracteristicas = caracteristicas,
        Preferencias = preferencias,
        Presupuesto = presupuesto,
        Frecuencia = frecuencia
    };

 // Guardar en base de datos
    using (var db = new SqlConnection("tu_cadena_de_conexion"))
    {
        string sql = @"INSERT INTO InfoUsuarios (IdUsuario, Caracteristicas, Preferencias, Presupuesto, Frecuencia)
                       VALUES (@IdUsuario, @Caracteristicas, @Preferencias, @Presupuesto, @Frecuencia)";

        db.Execute(sql, new
        {
            IdUsuario = info.IdUsuario,
            Caracteristicas = JsonConvert.SerializeObject(info.Caracteristicas),
            Preferencias = JsonConvert.SerializeObject(info.Preferencias),
            Presupuesto = info.Presupuesto,
            Frecuencia = info.Frecuencia
        });
    }

    // Guardar en sesión si querés usar después
    HttpContext.Session.SetString("infoUsuario", Objeto.ObjectToString(info));

    // Redirigir o mostrar mensaje
    return RedirectToAction("RutinaPersonalizada", "Home");
}



        Rutina rutina = Objeto.StringToObject<Rutina>(HttpContext.Session.GetString("rutina"));
        
        HttpContext.Session.SetString("rutina", Objeto.ObjectToString(rutina));

    }


}

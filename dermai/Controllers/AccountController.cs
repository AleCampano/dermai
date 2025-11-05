using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View("InicioSesion");
    }

    public IActionResult Comenzar(string Nombre, string Email, string Contraseña, DateTime FechaDeNacimiento, int IdPerfil)
    {
        Usuario usu = new Usuario (Nombre, Email, Contraseña, FechaDeNacimiento, IdPerfil);
        HttpContext.Session.SetString("usu", Objeto.ObjectToString(usu));
        return View("InicioSesion");
    }

    [HttpPost]
    public IActionResult GuardarLogin(string Email, string Contraseña)
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Contraseña))
        {
            ViewBag.Error = "Por favor, complete todos los campos.";
            return View("InicioSesion");
        }

        Usuario usu = BD.Login(Email, Contraseña);

        if(usu == null)
        {
            ViewBag.No = "Tu usuario no existe, por favor registrate.";
            return View("InicioSesion");
        }
            
        HttpContext.Session.SetString("usuId", usu.Email);
        return RedirectToAction("CompletarFormularioPiel", "User");
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    
    [HttpGet]
    public IActionResult Registro()
    {
        return View("Registrarse");
    }

    [HttpPost]
    public IActionResult GuardarRegistro(string Nombre, string Email, string Contraseña, DateTime FechaDeNacimiento)
    {
        if (string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Contraseña) || FechaDeNacimiento == DateTime.MinValue)
        {
            ViewBag.Error = "Por favor, complete todos los campos.";
            return View("Registrarse");
        }

        if (BD.ValidarRegistro(Email))
        {
            ViewBag.Ya = "Este usuario ya está registrado, inicie sesión.";
            return View("InicioSesion");
        }

        Perfil newPerfil = new Perfil (null, null, null, null);
        int NuevoPerfil = BD.CrearPerfil(newPerfil);
        Usuario newUser = new Usuario (Nombre, Email, Contraseña, FechaDeNacimiento, NuevoPerfil);
        BD.Registrarse(newUser);

        HttpContext.Session.SetString("usu", JsonConvert.SerializeObject(newUser));
        return RedirectToAction("CompletarFormularioPiel", "User");
    }

}

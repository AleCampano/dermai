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
    [Route("/api/Account/Login")]
    public IActionResult LoginApi([FromBody] UsuarioLoginDTO login)
    {   
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Contraseña))
        {
            return BadRequest(new { error = "Por favor, complete todos los campos." });
        }

        Usuario usu = BD.Login(login.Email, login.Contraseña);

        if (usu == null)
        {
            return NotFound(new { error = "Tu usuario no existe, por favor registrate." });
        }

        HttpContext.Session.SetString("usu", usu.Email);
        return Ok(new { mensaje = "Login exitoso", redireccion = Url.Action("CompletarFormularioPiel", "User") });
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

        if (Contraseña.Length < 6)
        {
            ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
            return View("Registrarse");
        }

        int edad = (DateTime.Today - FechaDeNacimiento).Days / 365;

        if (edad < 13)
        {
            ViewBag.Error = "Debe tener al menos 13 años para registrarse.";
            return View("Registrarse");
        }

        if (BD.ValidarRegistro(Email))
        {
            ViewBag.Ya = "Este usuario ya está registrado, inicie sesión.";
            return View("InicioSesion");
        }

        Usuario newUser = new Usuario(Nombre, Email, Contraseña, FechaDeNacimiento, 0);
        int idUsuario = BD.Registrarse(newUser);

        Perfil newPerfil = new Perfil (idUsuario, "", "", "", "");
        int idPerfil = BD.CrearPerfil(idUsuario, newPerfil);
        BD.AsignarPerfilAUsuario(Email, idPerfil);

        HttpContext.Session.SetString("usu", Objeto.ObjectToString(newUser));
        return RedirectToAction("CompletarFormularioPiel", "User");
    }

}

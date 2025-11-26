using Microsoft.AspNetCore.Mvc;
using dermai.Models;

namespace dermai.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        public IActionResult CompletarFormularioPiel()
        {
            return View("HacerMiRutina");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarFormularioPiel(int NivelGrasaPiel, string AlergiaProductos, string IrritacionFrecuencia, string AparicionGranos)
        {
            string tipoPiel = "Piel grasa";
            if (NivelGrasaPiel < 33) tipoPiel = "Piel seca";
            else if (NivelGrasaPiel < 66) tipoPiel = "Piel mixta";

            string detalles = tipoPiel + ", Alergia: " + AlergiaProductos +
                              ", Irritación: " + IrritacionFrecuencia +
                              ", Granos: " + AparicionGranos;

            string email = HttpContext.Session.GetString("usu");
            Usuario usuario = BD.ObtenerUsuarioPorEmail(email);

            if (usuario == null)
                return RedirectToAction("Login", "Account");

            Perfil perfil = new Perfil(detalles, "", "", "");

            if (usuario.IdPerfil > 0)
                BD.ModificarPerfil(usuario.IdPerfil, perfil);
            else
            {
                int idUsuario = BD.ObtenerIdUsuarioPorEmail(usuario.Email);
                int idPerfil = BD.CrearPerfil(idUsuario, perfil);
                BD.AsignarPerfilAUsuario(usuario.Email, idPerfil);
            }

            TempData["Mensaje"] = "Datos guardados correctamente";
            TempData["Mensaje2"] = tipoPiel;

            return RedirectToAction("InicioA", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarFormularioRutina(PielFormModel model)
        {
            string email = HttpContext.Session.GetString("usu");
            Usuario usuario = BD.ObtenerUsuarioPorEmail(email);

            if (usuario == null)
                return RedirectToAction("Login", "Account");

            string carac = Objeto.ListToString(model.Caracteristicas);
            string pref = Objeto.ListToString(model.Preferencias);

            Perfil perfil = new Perfil(carac, pref, model.Presupuesto, model.Frecuencia);

            int idPerfil;
            if (usuario.IdPerfil > 0)
            {
                BD.ModificarPerfil(usuario.IdPerfil, perfil);
                idPerfil = usuario.IdPerfil;
            }
            else
            {
                int idUsuario = BD.ObtenerIdUsuarioPorEmail(usuario.Email);
                idPerfil = BD.CrearPerfil(idUsuario, perfil);
                BD.AsignarPerfilAUsuario(usuario.Email, idPerfil);
            }

            return RedirectToAction("GenerarRutina", "Home", new { IdPerfil = idPerfil });
        }

        public IActionResult IrInicio()
        {
            return RedirectToAction("InicioA", "Home");
        }
    }
}
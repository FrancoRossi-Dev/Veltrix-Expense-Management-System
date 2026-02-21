using client.Filters;
using Domain;
using Domain.Pagos;
using Domain.Usuarios;
using Domain.Usuarios.Roles;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace client.Controllers
{
    public class UsuarioController : Controller
    {
        Sistema s = Sistema.GetSistema();

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("loggedUser") == null)
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Login(string userEmail, string password)
        {
            try
            {
                Usuario user = s.FindUserByMailAndPassword(userEmail, password);

                string userString = user.Nombre + ' ' + user.Apellido;
                HttpContext.Session.SetString("loggedUser", userString);
                HttpContext.Session.SetInt32("loggedUserId", user.Id); 

                string rolesString = "";
                foreach (Rol r in user.GetRoles())
                {
                    rolesString += r.ToString() + ' ';
                }
                HttpContext.Session.SetString("loggedUserRoles", rolesString);

                return RedirectToAction("Index", "Pago");
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [UserLoggedFilter]
        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            string? roles = HttpContext.Session.GetString("loggedUserRoles");
         
            Usuario user = s.getUserById(id);
            decimal totalThisMonth = s.CalcTotalOfMonth(user, DateTime.Now);
            ViewData["totalThisMonth"] = totalThisMonth;
            
            if (roles != null && roles.Contains("Gerente"))
            {
                IEnumerable<Usuario> usuariosDelEquipo = s.GetUsuariosPorEquipo(user.Equipo.Nombre);
                ViewData["ListaEquipo"] = usuariosDelEquipo;
            }

            return View(user);
        }

        [HttpPost]
        [UserLoggedFilter]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

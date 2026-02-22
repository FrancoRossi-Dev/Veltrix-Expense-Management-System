using client.Filters;
using Domain;
using Domain.Pagos;
using Domain.Usuarios;
using Domain.Usuarios.Roles;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using webApp.DTO;
using webApp.Mappers;
using webApp.ViewModels;

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
            UsuarioViewModel UsuarioVM = new();
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            string? roles = HttpContext.Session.GetString("loggedUserRoles");
            
            Usuario user = s.getUserById(id);
            UsuarioVM.Usuario = UsuarioMapper.ToDto(user);

            UsuarioVM.TotalThisMonth = s.CalcTotalOfMonth(user, DateTime.Now);


            if (roles != null && roles.Contains("Gerente"))
            {
                IEnumerable<Usuario> usuariosDelEquipo = s.GetUsuariosPorEquipo(user.Equipo.Nombre);
                IEnumerable<UsuarioDto> usuariosDelEquipoDto = usuariosDelEquipo.Select((u) => UsuarioMapper.ToDto(u));
                UsuarioVM.UsuariosDelEquipo = usuariosDelEquipoDto;
            }

            return View(UsuarioVM);
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

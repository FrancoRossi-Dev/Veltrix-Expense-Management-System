using client.Filters;
using Domain;
using Domain.Pagos;
using Microsoft.AspNetCore.Mvc;
using webApp.ViewModels;

namespace client.Controllers
{
    [UserLoggedFilter]
    [UserHasAccessFilter("Gerente")]
    public class TipoDeGastoController : Controller
    {
        Sistema s = Sistema.Instance;
        public IActionResult Index()
        {
            List<TipoDeGasto> g = s.GetTipoDeGastosActivos();
            TipoDeGastoViewModel TipoDeGastoVM = new();
            TipoDeGastoVM.TiposDeGasto = g.Count > 0 ? g : null;
            return View(TipoDeGastoVM);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TipoDeGasto t)
        {
            try
            {
                t.Validate();
                s.AltaTipoDeGasto(t);

                return Json(new
                {
                    success = true,
                    message = "Tipo de gasto creado exitosamente",
                    id = t.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public IActionResult Edit(int id)
        {

            try
            {
                TipoDeGasto t = s.FindTipoDeGastoById(id);
                return View(t);

            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Edit(string nombre, string descripcion, int Id)
        {
            TipoDeGasto? tOriginal = null;
            try
            {
                TipoDeGasto tempT = new(nombre, descripcion);
                tempT.Validate();

                tOriginal = s.FindTipoDeGastoById(Id);
                tOriginal.Nombre = nombre;
                tOriginal.Descripcion = descripcion;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                tOriginal = s.FindTipoDeGastoById(Id);
                ViewBag.msg = ex.Message;
                return View(tOriginal);
            }
        }

        [HttpPost]
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                TipoDeGasto tipo = s.FindTipoDeGastoById(id);
                if (s.TipoDeGastoInUse(tipo))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Tipo de gasto en uso"
                    });
                }

                tipo.Deactivate();
                return Json(new
                {
                    success = true,
                    message = $"Tipo de gasto {tipo.Nombre} eliminado correctamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}


using client.Filters;
using Domain;
using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.RegularExpressions;
using webApp.DTO;

namespace client.Controllers
{
    [UserLoggedFilter]

    public class PagoController : Controller
    {
        Sistema s = Sistema.GetSistema();

        [UserHasAccessFilter("Empleado")]
        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("loggedUserId");

            Usuario u = s.getUserById(id);

            List<TipoDeGasto> tiposDeGastoEnSistema = s.GetTipoDeGastosActivos();
            if (tiposDeGastoEnSistema.Count == 0) 
            { 
                ViewBag.TiposStatus = "ListaVacia"; 
            } 
            else 
            { 
                ViewData["tiposDeGastoEnSistema"] = tiposDeGastoEnSistema;
            }

            DateTime hoy = DateTime.Now;
            IEnumerable<(string Month, decimal Total)> totalsLastMoths = s.GetTotalsLastMonths(u, hoy);
            //double totalThisMonth = s.CalcTotalOfMonth(u, hoy);
            var culture = new CultureInfo("es-UY");

            IEnumerable<MonthlyTotalDto> monthlyTotals =
                totalsLastMoths.Select(x => new MonthlyTotalDto
                {
                    Month = x.Month,
                    Total = x.Total
                });


            Decimal totalThisMonth = monthlyTotals.Last().Total;
            ViewData["totalThisMonth"] = totalThisMonth;
            ViewData["totalsLastMonth"] = monthlyTotals;

            List<Pago> pagos = s.GetPagosByUserByMonth(u, hoy);
            if (pagos.Count == 0) ViewBag.ListStatus = "ListaVacia";

            ViewBag.pagosDelMes = pagos;
            return View();
        }

        [UserHasAccessFilter("Gerente")]
        public IActionResult Equipo()
        {
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            Usuario gerente = s.getUserById(id);

            DateTime hoy = DateTime.Now;
            List<Pago> pagosDelEquipo = s.GetPagosByTeamByMonth(gerente.Equipo.Id, hoy);

            string status = pagosDelEquipo.Count == 0 ? "ListaVacia" : "ok";
            double TotalEquipo = s.GetTotalPagosByList(pagosDelEquipo);

            ViewData["totalThisMonth"] = TotalEquipo;
            ViewData["status"] = status;
            ViewData["fecha"] = hoy;
            return View(pagosDelEquipo);
        }

        [HttpPost]
        [UserHasAccessFilter("Gerente")]
        public IActionResult Equipo(DateTime fecha)
        {
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            Usuario gerente = s.getUserById(id);

            List<Pago> pagosDelEquipo = s.GetPagosByTeamByMonth(gerente.Equipo.Id, fecha);
            double TotalEquipo = s.GetTotalPagosByList(pagosDelEquipo);

            string status = pagosDelEquipo.Count == 0 ? "ListaVacia" : "ok";

            ViewData["totalThisMonth"] = TotalEquipo;
            ViewData["status"] = status;
            ViewData["fecha"] = fecha;
            return View(pagosDelEquipo);
        }

        [HttpPost]
        [UserHasAccessFilter("Empleado")]
        public IActionResult Create(string typeOfPayment, double MontoInicial, int TipoGastoId, MetodoDePago Metodo, string Descripcion, DateTime FechaDePago, string NroRecibo, DateTime PrimerPago, bool esIndefinida, int cuotas)
        {
            TipoDeGasto TipoGasto = s.FindTipoDeGastoById(TipoGastoId);

            int? userId = HttpContext.Session.GetInt32("loggedUserId");
            Usuario usuario = s.getUserById(userId);

            if (TipoGasto == null || usuario == null)
            {
                return Json(new
                {
                    state = "error",
                    msg = "tipo de gasto o usuario desconocido"
                });
            }
            try
            {
                if (typeOfPayment == "unico")
                {
                    Unico u = new(MontoInicial, Metodo, TipoGasto, usuario, Descripcion, FechaDePago, NroRecibo);
                    return CreateUnico(u);
                }

                if (typeOfPayment == "recurrente")
                {
                    if (esIndefinida)
                    {
                        Recurrente r = new(MontoInicial, Metodo, TipoGasto, usuario, Descripcion, PrimerPago);
                        return CreateRecurrente(r);
                    }
                    else
                    {
                        DateTime UltimoPago = PrimerPago.AddMonths(cuotas);
                        Console.WriteLine(UltimoPago);
                        Recurrente r = new(MontoInicial, Metodo, TipoGasto, usuario, Descripcion, PrimerPago, UltimoPago);
                        return CreateRecurrente(r);
                    }
                }
            }
            catch (Exception ex)
            {


                return Json(new
                {
                    state = "error",
                    msg = ex.Message
                });
            }


            return Json(new
            {
                state = "error",
                msg = "Tipo de pago desconocido"
            });
        }


        private IActionResult CreateUnico(Unico u)
        {
            try
            {
                u.Validate();
                s.AltaPago(u);

                return Json(new
                {
                    TipoDePago = u.TipoDePago(),
                    Descripcion = u.Descripcion,
                    FechaPago = u.MiFechaDePago(),
                    TipoDeGasto = u.TipoGasto.Nombre,
                    Metodo = u.Metodo.ToString(),
                    MontoTotal = u.MontoTotal,
                    Modificador = u.MiModificador(),
                    state = "exito",
                    msg = "Pago creado con exito"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = "error",
                    msg = ex.Message
                });
            }
        }


        private IActionResult CreateRecurrente(Recurrente r)
        {

            if (r != null)
            {
                r.Validate();
                s.AltaPago(r);

                return Json(new
                {
                    TipoDePago = r.TipoDePago(),
                    Descripcion = r.Descripcion,
                    FechaPago = r.MiFechaDePago(),
                    TipoDeGasto = r.TipoGasto.Nombre,
                    Metodo = r.Metodo.ToString(),
                    MontoTotal = r.MontoTotal,
                    Modificador = r.MiModificador(),
                    state = "exito",
                    msg = "Pago creado con exito"
                });
            }

            return Json(new
            {
                state = "error",
                msg = "Error en el controlador"
            });
        }
    }
}

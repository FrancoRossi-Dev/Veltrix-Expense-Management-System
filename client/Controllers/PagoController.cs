
using client.Filters;
using Domain;
using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using webApp.DTO;
using webApp.ViewModels;

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

            DateTime hoy = DateTime.Now;
            IEnumerable<(string Month, decimal Total)> totalsLastMoths = s.GetTotalsLastMonths(u, hoy);
            var culture = new CultureInfo("es-UY");

            IEnumerable<MonthlyTotalDto> monthlyTotals =
                totalsLastMoths.Select(x => new MonthlyTotalDto
                {
                    Month = x.Month,
                    Total = x.Total
                });

            Decimal totalThisMonth = monthlyTotals.Last().Total;

            IEnumerable<Pago> pagosDelMes = s.GetPagosByUserByMonth(u, hoy);
            IEnumerable<PagoDto> pagosDelMesDto = pagosDelMes.Select(p => PagoMapper.ToDto(p));

            MonthExpensesDto monthExpensesDto = new();
            monthExpensesDto.Expenses = totalThisMonth;
            monthExpensesDto.MyBudget = u.CalcPersonalBudget();

            PagoViewModel pvm = new();
            pvm.PagosDelMes = pagosDelMesDto;
            pvm.TiposDeGastoEnSistema = s.GetTipoDeGastosActivos();
            pvm.TotalsLastMonths = monthlyTotals;
            pvm.TotalThisMonth = totalThisMonth;
            pvm.MonthExpenses = monthExpensesDto;
            return View(pvm);
        }

        [UserHasAccessFilter("Gerente")]
        public IActionResult Equipo()
        {
            EquipoViewModel EquipoVM = new();
            
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            Usuario gerente = s.getUserById(id);

            DateTime hoy = DateTime.Now;
            List<Pago> pagosDelEquipo = s.GetPagosByTeamByMonth(gerente.Equipo.Id, hoy);

            if (pagosDelEquipo.Count == 0)
            {
                EquipoVM.PagosDelEquipo = null;
            }
            else
            {
                IEnumerable<PagoDto> pagosDelEquipoDto = pagosDelEquipo.Select(x => PagoMapper.ToDto(x));
                EquipoVM.PagosDelEquipo = pagosDelEquipoDto;
            }

            MonthExpensesDto monthExpensesDto = new();
            monthExpensesDto.Expenses = s.GetTotalPagosByList(pagosDelEquipo);
            monthExpensesDto.MyBudget = s.CalcTeamBudget(gerente.Equipo.Id);

            EquipoVM.MonthExpenses = monthExpensesDto;
            return View(EquipoVM);
        }

        [HttpPost]
        [UserHasAccessFilter("Gerente")]
        public IActionResult Equipo(DateTime fecha)
        {
            EquipoViewModel EquipoVM = new();
            int? id = HttpContext.Session.GetInt32("loggedUserId");
            Usuario gerente = s.getUserById(id);

            List<Pago> pagosDelEquipo = s.GetPagosByTeamByMonth(gerente.Equipo.Id, fecha);

            if (pagosDelEquipo.Count == 0)
            {
                EquipoVM.PagosDelEquipo = null;
            }
            else
            {
                IEnumerable<PagoDto> pagosDelEquipoDto = pagosDelEquipo.Select(x => PagoMapper.ToDto(x));
                EquipoVM.PagosDelEquipo = pagosDelEquipoDto;
            }

            MonthExpensesDto monthExpensesDto = new();
            monthExpensesDto.Expenses = s.GetTotalPagosByList(pagosDelEquipo);
            monthExpensesDto.MyBudget = s.CalcTeamBudget(gerente.Equipo.Id);
            EquipoVM.MonthExpenses = monthExpensesDto;

            return View(EquipoVM);
        }

        [HttpPost]
        [UserHasAccessFilter("Empleado")]
        public IActionResult Create(string typeOfPayment, decimal MontoInicial, int TipoGastoId, MetodoDePago Metodo, string Descripcion, DateTime FechaDePago, string NroRecibo, DateTime PrimerPago, int cuotas)
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

                if (typeOfPayment == "subscripcion")
                {
                    Subscripcion sub = new(MontoInicial, Metodo, TipoGasto, usuario, Descripcion, PrimerPago);
                    return CreateSubscription(sub);
                }

                if (typeOfPayment == "cuotas")
                {
                    DateTime UltimoPago = PrimerPago.AddMonths(cuotas);
                    Console.WriteLine(UltimoPago);
                    Cuotas c = new(MontoInicial, Metodo, TipoGasto, usuario, Descripcion, PrimerPago, UltimoPago);
                    return CreateCuotas(c);
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

        private IActionResult CreateCuotas(Cuotas c)
        {

            if (c != null)
            {
                c.Validate();
                s.AltaPago(c);

                return Json(new
                {
                    TipoDePago = c.TipoDePago(),
                    Descripcion = c.Descripcion,
                    FechaPago = c.MiFechaDePago(),
                    TipoDeGasto = c.TipoGasto.Nombre,
                    Metodo = c.Metodo.ToString(),
                    MontoTotal = c.MontoTotal,
                    Modificador = c.MiModificador(),
                    state = "exito",
                    msg = "Pago en cuotas ingresado con exito"
                });
            }

            return Json(new
            {
                state = "error",
                msg = "Error en el controlador"
            });
        }

        private IActionResult CreateSubscription(Subscripcion sub)
        {

            if (sub != null)
            {
                sub.Validate();
                s.AltaPago(sub);

                return Json(new
                {
                    TipoDePago = sub.TipoDePago(),
                    Descripcion = sub.Descripcion,
                    FechaPago = sub.MiFechaDePago(),
                    TipoDeGasto = sub.TipoGasto.Nombre,
                    Metodo = sub.Metodo.ToString(),
                    MontoTotal = sub.MontoTotal,
                    Modificador = sub.MiModificador(),
                    state = "exito",
                    msg = "Subscripcion ingresada con exito"
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

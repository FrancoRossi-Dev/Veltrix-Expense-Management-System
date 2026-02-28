
using client.Filters;
using Domain;
using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
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
            if (u == null)
            {
                return Json(new
                {
                    state = "error",
                    msg = "Datos inválidos"
                });
            }

            try
            {
                u.Validate();

                BudgetStatus BStatus = s.ValidateBudget(u);
                if (BStatus == BudgetStatus.Over) return Json(new
                {
                    state = "error",
                    msg = "El pago supera el presupuesto menusal"
                });
                s.AltaPago(u);
                string state = BStatus == BudgetStatus.Allowed ? "success" : "warning";
                string msg = BStatus == BudgetStatus.Allowed ? "Pago ingresado con exito" : "Pago ingresado, cerca del límite";
                return Json(MapPagoResponse(u, state, msg));

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

            if (c == null)
            {
                return Json(new
                {
                    state = "error",
                    msg = "Datos inválidos"
                });
            }
            try
            {
                c.Validate();
                BudgetStatus BStatus = s.ValidateBudget(c);
                if (BStatus == BudgetStatus.Over) return Json(new
                {
                    state = "error",
                    msg = "El pago supera el presupuesto menusal"
                });
                s.AltaPago(c);
                string state = BStatus == BudgetStatus.Allowed ? "success" : "warning";
                string msg = BStatus == BudgetStatus.Allowed ? "Pago en cuotas ingresado con exito" : "Pago ingresado, cerca del límite";
                return Json(MapPagoResponse(c, state, msg));

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = "error",
                    msg = ex.Message,
                });
            }

        }

        private IActionResult CreateSubscription(Subscripcion sub)
        {
            if (sub == null)
            {
                return Json(new
                {
                    state = "error",
                    msg = "Datos inválidos"
                });
            }

            try
            {
                sub.Validate();
                BudgetStatus BStatus = s.ValidateBudget(sub);
                if (BStatus == BudgetStatus.Over) return Json(new
                {
                    state = "error",
                    msg = "El pago supera el presupuesto menusal"
                });
                s.AltaPago(sub);
                string state = BStatus == BudgetStatus.Allowed ? "success" : "warning";
                string msg = BStatus == BudgetStatus.Allowed ? "Subscripción ingresada con exito" : "Pago ingresado, cerca del límite";
                return Json(MapPagoResponse(sub, state, msg));
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

        private object MapPagoResponse(Pago p, string state, string msg)
        {
            return new
            {
                TipoDePago = p.TipoDePago(),
                Descripcion = p.Descripcion,
                FechaPago = p.MiFechaDePago(),
                TipoDeGasto = p.TipoGasto.Nombre,
                Metodo = p.Metodo.ToString(),
                MontoTotal = p.MontoTotal,
                Modificador = p.MiModificador(),
                state,
                msg
            };
        }
    }
}

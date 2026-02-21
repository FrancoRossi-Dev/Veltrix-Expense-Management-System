using Domain.Pagos;
using webApp.DTO;

namespace webApp.ViewModels
{
    public class PagoViewModel
    {
        public string Title { get; set; } = "Mis Pagos";

        public IEnumerable<PagoDto>? PagosDelMes {  get; set; } = null;

        public IEnumerable<TipoDeGasto>? TiposDeGastoEnSistema { get; set; } = null;

        public IEnumerable<MonthlyTotalDto>? TotalsLastMonths {  get; set; }

        public decimal TotalThisMonth { get; set; }

        public MonthExpensesDto? MonthExpenses { get; set;}
    }
}

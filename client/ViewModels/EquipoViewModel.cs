using webApp.DTO;

namespace webApp.ViewModels
{
    public class EquipoViewModel
    {
        public string Title { get; set; } = "Pagos del equipo";

        public MonthExpensesDto? MonthExpenses { get; set;}
        public IEnumerable<PagoDto>? PagosDelEquipo { get; set; }

        public DateTime? Fecha { get; set; } = DateTime.Now;
    }
}

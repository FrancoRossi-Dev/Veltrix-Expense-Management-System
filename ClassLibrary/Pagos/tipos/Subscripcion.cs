using Domain.Usuarios;

namespace Domain.Pagos.tipos
{
    public class Subscripcion : Pago
    {

        static private decimal RECARGO_SUBSCRIPCION = 0.03m;

        public DateTime PrimerPago { get; set; }
        public bool IsActive { get; set; }
        
        public DateTime CancelDate { get; set; }

        public Subscripcion()
        {
        }

        public Subscripcion(decimal montoInicial, MetodoDePago metodo, TipoDeGasto tipoGasto, Usuario usuario, string descripcion, DateTime primerPago) : base(montoInicial, metodo, tipoGasto, usuario, descripcion)
        {
            PrimerPago = primerPago;
            CancelDate = DateTime.MaxValue;
            IsActive = true;
            Validate();
            MontoTotal = CalcTotal();
        }

        public void CancelSubscription()
        {
            IsActive = false;
            CancelDate = DateTime.Now;
        } 


        public override decimal CalcTotal()
        { 
            decimal mod = 1 + RECARGO_SUBSCRIPCION;
            return Math.Round(MontoInicial * mod);
        }


        public override string ToString()
        {
            return base.ToString() + ", Indefinida recurrente";
        }

        public override void Validate()
        {
            base.Validate();
        }

        public override bool DelMes(DateTime? fecha)
        {
            DateTime f = fecha ?? DateTime.Now;
            DateTime inicioMes = new DateTime(f.Year, f.Month, 1);
            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);

            if (!IsActive && CancelDate < inicioMes) return false;

            return PrimerPago <= finMes;
        }

        public override string TipoDePago()
        {
            return "Subscripcion";
        }

        public override string MiFechaDePago()
        {
            string fecha = PrimerPago.ToShortDateString();
            return fecha;
        }

        public override string MiModificador()
        {
            decimal mod = RECARGO_SUBSCRIPCION;  
            string modificadorString = $"{Math.Ceiling(mod * 100)} %";
            return modificadorString;
        }
    }
}

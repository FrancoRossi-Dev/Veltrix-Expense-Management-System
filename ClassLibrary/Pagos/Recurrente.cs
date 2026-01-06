using Domain.Pagos;
using Domain.Usuarios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pagos
{
    public class Recurrente : Pago
    {
        static private double RECARGO_RECURRENTE_INDEFINIDO = 0.03;
        static private double RECARGO_RECURRENTE_MAX = 0.10;
        static private double RECARGO_RECURRENTE_MED = 0.05;
        static private double RECARGO_RECURRENTE_MIN = 0.03;

        public DateTime PrimerPago { get; set; }
        public DateTime UltimoPago { get; set; }
        public bool EsIndefinida { get; set; }

        public Recurrente()
        {
        }

        // compra indefinida
        public Recurrente(double montoInicial, MetodoDePago metodo, TipoDeGasto tipoGasto, Usuario usuario, string descripcion, DateTime primerPago) : base(montoInicial, metodo, tipoGasto, usuario, descripcion)
        {
            PrimerPago = primerPago;
            UltimoPago = DateTime.MaxValue;

            EsIndefinida = true;
            Validate();
            MontoTotal = CalcTotal();
        }

        // compra con fin
        public Recurrente(double montoInicial, MetodoDePago metodo, TipoDeGasto tipoGasto, Usuario usuario, string descripcion, DateTime primerPago, DateTime ultimoPago) : base(montoInicial, metodo, tipoGasto, usuario, descripcion)
        {
            PrimerPago = primerPago;
            UltimoPago = ultimoPago;

            EsIndefinida = false;

            Validate();
            MontoTotal = CalcTotal();
        }

        public override double CalcTotal()
        {
            double mod = 1;
            if (EsIndefinida)
            {
                mod += RECARGO_RECURRENTE_INDEFINIDO;
                return Math.Round(MontoInicial * mod);
            }
            else
            {
                int cuotas = CalcCuotas();
                if (cuotas >= 10) mod += RECARGO_RECURRENTE_MAX;
                if (cuotas <= 9 && cuotas >= 6) mod += RECARGO_RECURRENTE_MED;
                if (cuotas <= 5) mod += RECARGO_RECURRENTE_MIN;

                return Math.Round(MontoInicial * mod);
            }

        }

        private int CalcCuotas()
        {
            int months = UltimoPago.Month - PrimerPago.Month;
            int years = UltimoPago.Year - PrimerPago.Year;
            return months + (years * 12) + 1;
        }

        private int CalcCuotasRestantes()
        {
            int months = UltimoPago.Month - DateTime.Now.Month;
            int years = UltimoPago.Year - DateTime.Now.Year;
            return months + (years * 12) + 1;
        }

        public override string ToString()
        {
            if (EsIndefinida)
            {
                return base.ToString() + ", Indefinida recurrente";
            }

            int cuotas = CalcCuotasRestantes();
            if (cuotas > 0)
            {
                return $"{base.ToString()}, recurrente, cuotas faltantes: {cuotas}";
            }
            else
            {
                return $"{base.ToString()}, recurrente, pago finalizado en " +
                    $"{UltimoPago.ToString("D", new CultureInfo("es-UY"))}";
            }
        }

        public override void Validate()
        {
            base.Validate();
            ValidateFechas();
        }

        private void ValidateFechas()
        {
            if (PrimerPago > UltimoPago) throw new Exception("Error en las fechas, el primer pago debe ocurrir antes que el ultimo");
        }

        public override bool DelMes(DateTime? fecha)
        {
            if (fecha == null)
            {
                fecha = DateTime.Now;
            }
            DateTime inicioMesActual = new DateTime(fecha.Value.Year, fecha.Value.Month, 1);
            DateTime finMesActual = inicioMesActual.AddMonths(1).AddDays(-1);

            if (EsIndefinida)
            {
                return finMesActual >= PrimerPago;
            }
            else
            {
                return PrimerPago <= finMesActual && UltimoPago >= inicioMesActual;
            }
        }

        public override string TipoDePago()
        {
            return "Recurrente";
        }

        public override string MiFechaDePago()
        {
            string fecha = PrimerPago.ToShortDateString();
            return fecha;
        }

        public override string MiModificador()
        {
            double mod = 0;
            if (EsIndefinida)
            {
                mod += RECARGO_RECURRENTE_INDEFINIDO;
            }
            else
            {
                int cuotas = CalcCuotas();
                if (cuotas >= 10) mod += RECARGO_RECURRENTE_MAX;
                if (cuotas <= 9 && cuotas >= 6) mod += RECARGO_RECURRENTE_MED;
                if (cuotas <= 5) mod += RECARGO_RECURRENTE_MIN;
            }

            string modificadorString = $"%{Math.Ceiling((mod) * 100)}";
            return modificadorString;
        }
    }
}

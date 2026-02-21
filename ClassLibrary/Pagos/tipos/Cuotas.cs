using Domain.Usuarios;
using System;
using System.Globalization;

namespace Domain.Pagos.tipos
{
    public class Cuotas : Pago
    {
        private const decimal RECARGO_CUOTAS_MAX = 0.10m;
        private const decimal RECARGO_CUOTAS_MED = 0.05m;
        private const decimal RECARGO_CUOTAS_MIN = 0.03m;

        public DateTime PrimerPago { get; private set; }
        public DateTime UltimoPago { get; private set; }

        public bool IsActive => UltimoPago.Date >= DateTime.Now.Date;

        public Cuotas() { }

        public Cuotas(
            decimal montoInicial,
            MetodoDePago metodo,
            TipoDeGasto tipoGasto,
            Usuario usuario,
            string descripcion,
            DateTime primerPago,
            DateTime ultimoPago
        ) : base(montoInicial, metodo, tipoGasto, usuario, descripcion)
        {
            PrimerPago = primerPago.Date;
            UltimoPago = ultimoPago.Date;

            Validate();
            MontoTotal = CalcTotal();
        }

        public override decimal CalcTotal()
        {
            decimal modificador = 1 + ObtenerRecargo();
            return Math.Round(MontoInicial * modificador, 2, MidpointRounding.AwayFromZero);
        }


        public int CantidadCuotas => CalcularDiferenciaMeses(PrimerPago, UltimoPago) + 1;


        public int CuotasRestantes
        {
            get
            {
                if (!IsActive) return 0;

                int restantes = CalcularDiferenciaMeses(DateTime.Now.Date, UltimoPago) + 1;
                return Math.Max(0, restantes);
            }
        }

        private decimal ObtenerRecargo()
        {
            int cuotas = CantidadCuotas;

            if (cuotas >= 10) return RECARGO_CUOTAS_MAX;
            if (cuotas >= 6) return RECARGO_CUOTAS_MED;
            return RECARGO_CUOTAS_MIN;
        }

        private int CalcularDiferenciaMeses(DateTime inicio, DateTime fin)
        {
            int meses = (fin.Year - inicio.Year) * 12 + (fin.Month - inicio.Month);

            // Ajuste si el día del mes aún no se cumplió
            if (fin.Day < inicio.Day)
                meses--;

            return meses;
        }

        public override void Validate()
        {
            base.Validate();
            ValidarFechas();
        }

        private void ValidarFechas()
        {
            if (PrimerPago == default)
                throw new Exception("La fecha del primer pago es obligatoria.");

            if (UltimoPago == default)
                throw new Exception("La fecha del último pago es obligatoria.");

            if (PrimerPago > UltimoPago)
                throw new Exception("Error en las fechas: el primer pago debe ocurrir antes que el último.");
        }

        public override bool DelMes(DateTime? fecha)
        {
            DateTime refFecha = (fecha ?? DateTime.Now).Date;

            DateTime inicioMes = new DateTime(refFecha.Year, refFecha.Month, 1);
            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);

            return PrimerPago <= finMes && UltimoPago >= inicioMes;
        }

        public override string TipoDePago()
        {
            return "Cuotas";
        }

        public override string MiFechaDePago()
        {
            return PrimerPago.ToShortDateString();
        }

        public override string MiModificador()
        {
            decimal recargo = ObtenerRecargo();
            return $"{Math.Ceiling(recargo * 100)} %";
        }

        public override string ToString()
        {
            if (IsActive)
            {
                return $"{base.ToString()}, recurrente, cuotas faltantes: {CuotasRestantes}";
            }

            return $"{base.ToString()}, recurrente, pago finalizado en " +
                   $"{UltimoPago.ToString("D", new CultureInfo("es-UY"))}";
        }
    }
}
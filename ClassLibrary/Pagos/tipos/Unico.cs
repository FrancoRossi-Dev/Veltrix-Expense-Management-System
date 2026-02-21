using Domain.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pagos.tipos
{
    public class Unico : Pago
    {
        static internal decimal BENEFICIO_PAGO_UNICO = -0.10m;
        static internal decimal BENEFICIO_PAGO_EFECTIVO = -0.10m;

        public DateTime FechaDePago { get; set; }
        public string NroRecibo { get; set; }

        public Unico()
        {
        }

        public Unico(decimal montoInicial, MetodoDePago metodo, TipoDeGasto tipoGasto, Usuario usuario, string descripcion, DateTime fechaDePago, string nroRecibo) : base(montoInicial, metodo, tipoGasto, usuario, descripcion)
        {
            FechaDePago = fechaDePago;
            NroRecibo = nroRecibo;

            Validate();
            MontoTotal = CalcTotal();
        }

        public override decimal CalcTotal()
        {
            decimal modificador = 1;
            modificador += BENEFICIO_PAGO_UNICO;
            if (Metodo == MetodoDePago.EFECTIVO) modificador += BENEFICIO_PAGO_EFECTIVO;
            return Math.Round(MontoInicial * modificador);
        }

        public override bool DelMes(DateTime? fecha)
        {
            if (fecha == null)
            {
                fecha = DateTime.Now;
            }

            return FechaDePago.Month == fecha.Value.Month && FechaDePago.Year == fecha.Value.Year;
        }

        public override string ToString()
        {
            return base.ToString() + $", numero de recibo: {NroRecibo}.";
        }

        public override void Validate()
        {
            base.Validate();
            ValidateRecibo();
            ValidateFecha();
        }

        private void ValidateFecha()
        {
            DateTime hoy = DateTime.Now;

            if (FechaDePago > hoy)
                throw new Exception("fecha invalida, la fecha debe estar en el pasado");
        }

        private void ValidateRecibo()
        {
            if (string.IsNullOrEmpty(NroRecibo)) throw new Exception("Debe ingresar un numero de recibo");
        }

        public override string TipoDePago()
        {
            return "Único";
        }

        public override string MiFechaDePago()
        {
            string fecha = FechaDePago.ToShortDateString();
            return fecha;
        }

        public override string MiModificador()
        {
            decimal modificador = 1;
            modificador += BENEFICIO_PAGO_UNICO;
            if (Metodo == MetodoDePago.EFECTIVO) modificador += BENEFICIO_PAGO_EFECTIVO;
            string modificadorString = $"-{Math.Ceiling((1 - modificador) * 100)} %";
            return modificadorString;
        }
    }
}
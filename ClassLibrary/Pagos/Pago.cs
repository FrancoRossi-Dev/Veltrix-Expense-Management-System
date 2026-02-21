using Domain.Pagos;
using Domain.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pagos
{
    abstract public class Pago : IValidable, IComparable<Pago>
    {

        public int Id { get; set; }
        static public int IdCount { get; set; } = 1;

        public decimal MontoInicial { get; set; }

        public MetodoDePago Metodo { get; set; }

        public TipoDeGasto TipoGasto { get; set; }

        public Usuario Usuario { get; set; }

        public string Descripcion { get; set; }
        public decimal MontoTotal { get; set; }

        public Pago()
        {
            Id = IdCount++;
        }

        protected Pago(decimal montoInicial, MetodoDePago metodo, TipoDeGasto tipoGasto, Usuario usuario, string descripcion)
        {
            Id = IdCount++;
            MontoInicial = montoInicial;
            Metodo = metodo;
            TipoGasto = tipoGasto;
            Usuario = usuario;
            Descripcion = descripcion;
        }
        
        public override string ToString()
        {
            return $"id: {Id}, item: {Descripcion}, metodo de pago: {Metodo}, total: {MontoTotal:F2}";
        }

        virtual public void Validate()
        {
            ValidateDescripcion();
            ValidateMonto();
        }

        private void ValidateMonto()
        {
            if (MontoInicial < 0) throw new Exception("No puede ingresar un monto negativo");
        }

        private void ValidateDescripcion()
        {
            if (String.IsNullOrEmpty(Descripcion)) 
                throw new ArgumentException("Se debe describir el gasto");
        }
        public int CompareTo(Pago other)
        {
            int result = -MontoTotal.CompareTo(other.MontoTotal);
            if (result != 0) return result;
            return Id.CompareTo(other.Id);
        }
        abstract public bool DelMes(DateTime? fecha);
        abstract public decimal CalcTotal();

        public abstract string TipoDePago();
        public abstract string MiFechaDePago();
        public abstract string MiModificador();
    }
}

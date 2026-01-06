using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Pagos
{
    public class TipoDeGasto : IValidable, IComparable<TipoDeGasto>
    {
        public int Id { get; set; }
        static public int IdCount { get; set; } = 1;
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool IsActive { get; set; } = true;

        public TipoDeGasto()
        {
            Id = IdCount++;
        }

        public TipoDeGasto(string nombre, string descripcion)
        {
            Id = IdCount++;
            Nombre = nombre;
            Descripcion = descripcion;
            Validate();
        }

        public override string ToString()
        {
            return $"Tipo de gasto: {Nombre}";
        }


        public void Validate()
        {
            ValidateNombre();
            ValidateDescipcion();
        }

        private void ValidateNombre()
        {
            if (String.IsNullOrEmpty(Nombre)) 
                throw new Exception("el nombre no puede ser vacio");
        }

        private void ValidateDescipcion()
        {
            if (String.IsNullOrEmpty(Descripcion))
                throw new Exception("la descripcion no puede ser vacia");
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public int CompareTo(TipoDeGasto? other)
        {
            if (other == null) return 0;
            return Nombre.CompareTo(other.Nombre);
        }
    }
}

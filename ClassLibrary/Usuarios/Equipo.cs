using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Usuarios
{
    public class Equipo : IValidable
    {

        public int Id { get; set; }
        static public int IdCount { get; set; } = 1;
        public string Nombre { get; set; }

        public decimal Budget { get; set; }

        public Equipo()
        {
            Id = IdCount++;
        }

        public Equipo(string nombre, decimal budget)
        {
            Id = IdCount++;
            Nombre = nombre;
            Budget = budget;
            Validate();
        }

        public override string ToString()
        {
            return $"{Nombre}";
        }

        public void Validate()
        {
            ValidateNombre();
            ValidateBudget();
        }

        private void ValidateBudget()
        {
            if (Budget < 0) throw new Exception
                    ("No se puede ingresar un presupuesto negativo");
        }

        private void ValidateNombre()
        {
            if (String.IsNullOrEmpty(Nombre)) throw new Exception
                    ("El nombre del equipo no puede ser vacio");
        }
    }
}

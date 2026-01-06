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

        public Equipo()
        {
            Id = IdCount++;
        }

        public Equipo(string nombre)
        {
            Id = IdCount++;
            Nombre = nombre;
            Validate();
        }

        public override string ToString()
        {
            return $"{Nombre}";
        }

        public void Validate()
        {
            ValidateNombre();
        }

        private void ValidateNombre()
        {
            if (String.IsNullOrEmpty(Nombre)) throw new Exception
                    ("El nombre del equipo no puede ser vacio");
        }
    }
}

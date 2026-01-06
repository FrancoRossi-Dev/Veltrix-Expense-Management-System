using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Usuarios.Roles
{
    abstract public class Rol
    {
        public int Id { get; set; }
        static private int IdCount { get; set; } = 1;
        public string Titulo { get; set; }

        public Rol()
        {
            Id = IdCount++;
        }

        public Rol(string titulo)
        {
            Id = IdCount++;
            Titulo = titulo;
        }


        public override string ToString()
        {
            return Titulo;
        }
    }
}

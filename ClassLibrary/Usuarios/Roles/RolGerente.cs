using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Usuarios.Roles
{
    internal class RolGerente : Rol
    {

        public RolGerente() : base()
        {
            Titulo = "Gerente";
        }

    }
}

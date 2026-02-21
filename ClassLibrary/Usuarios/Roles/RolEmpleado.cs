using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Usuarios.Roles
{
    internal class RolEmpleado : Rol
    {
        public RolEmpleado() : base()
        {
            Titulo = "Empleado";
            BudgetMod = 1m;
        }

    }
}

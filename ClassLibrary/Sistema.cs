using Domain.data;
using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Domain.Usuarios.Roles;
using System.Globalization;


namespace Domain
{
    public class Sistema
    {
        private Sistema()
        {
        }

        static private Sistema? _sistema = null;
        static public Sistema GetSistema()
        {
            if (_sistema == null)
            {
                _sistema = new Sistema();
                Preload preload = new();
                preload.Precarga();

            }
            return _sistema;
        }

        #region Lists

        // Tipos de gasto
        private List<TipoDeGasto> _tiposDeGastos = new List<TipoDeGasto>();
        public List<TipoDeGasto> GetTipoDeGastos()
        {
            return _tiposDeGastos;
        }
        public List<TipoDeGasto> GetTipoDeGastosActivos()
        {
            List<TipoDeGasto> tipoDeGastos = GetTipoDeGastos();
            List<TipoDeGasto> activos = new();

            foreach (TipoDeGasto t in tipoDeGastos)
            {
                if (t.IsActive) activos.Add(t);
            }
            activos.Sort();
            return activos;
        }

        public void AltaTipoDeGasto(TipoDeGasto t)
        {
            if (t == null) throw new Exception("no se puede agregar un tipo de gasto nulo");
            if (_tiposDeGastos.Contains(t))
            {
                TipoDeGasto tOriginal = FindTipoDeGastoById(t.Id);
                if (tOriginal.IsActive)
                {
                    throw new Exception("El tipo de gasto ya ha sido ingresado");
                }
                else
                {
                    tOriginal.Activate();
                }
            }
            else
            {
                _tiposDeGastos.Add(t);
            }
        }

        // Equipos
        private List<Equipo> _equipos = new List<Equipo>();
        public List<Equipo> GetEquipos() { return _equipos; }
        public void AltaEquipo(Equipo e)
        {
            try
            {
                if (e == null) throw new Exception
                        ("No se puede dar de alta un equipo nulo.");
                if (_equipos.Contains(e)) throw new Exception
                        ("El equipo ya fue registrado");
                _equipos.Add(e);
            }
            catch
            {
                throw;
            }

        }
        // Roles
        private List<Rol> _roles = new List<Rol>();

        public void AltaRol(Rol r)
        {
            _roles.Add(r);
        }

        public List<Rol> GetRoles() { return _roles; }


        // Usuarios
        private List<Usuario> _usuarios = new List<Usuario>();
        public List<Usuario> GetUsuarios() { return _usuarios; }
        public void AltaUsuario(Usuario u)
        {
            // no se revisa si ya existe el usuario, no hay un atributo unico
            try
            {
                if (u == null) throw new Exception("Se ha ingresado un usuario nulo");
                u.Validate();
                u.GenerateEmail(_usuarios);
                _usuarios.Add(u);
            }
            catch
            {
                throw;
            }

        }

        // Pagos
        private List<Pago> _pagos = new List<Pago>();
        public List<Pago> GetPagos() { return _pagos; }
        public void AltaPago(Pago p)
        {
            try
            {
                if (p == null) throw new Exception("Se ha ingresado un pago nulo");
                if (_pagos.Contains(p)) throw new Exception("El pago ya fue ingresado");

                _pagos.Add(p);
            }
            catch
            {
                throw;
            }

        }

        #endregion

        public List<Pago> GetPagosDelMes(DateTime? fecha)
        {

            List<Pago> match = new();
            foreach (Pago pago in _pagos)
            {
                if (pago.DelMes(fecha)) match.Add(pago);
            }
            return match;
        }


        public List<Usuario> GetUsuariosPorEquipo(string equipoNombre)
        {
            List<Usuario> usuariosDelEquipo = new List<Usuario>();
            foreach (Usuario us in _usuarios)
            {
                if (us.PertenceA(equipoNombre)) usuariosDelEquipo.Add(us);
            }
            usuariosDelEquipo.Sort();
            return usuariosDelEquipo;
        }

        public Usuario? FindUserByMail(string inputMail)
        {
            Usuario? match = null;

            if (string.IsNullOrEmpty(inputMail)) throw new Exception("Email vacio");

            foreach (Usuario usuario in _usuarios)
            {
                if (usuario.Email.ToLower().Equals(inputMail.ToLower()))
                    match = usuario;
            }

            return match;
        }

        public Usuario FindUserByMailAndPassword(string userMail, string password)
        {
            if (String.IsNullOrEmpty(userMail) || String.IsNullOrEmpty(password))
                throw new Exception("Complete ambos campos");

            Usuario? u = FindUserByMail(userMail);

            if (u == null || u.Contrasenia != password)
                throw new Exception("Credenciales incorrectas");

            return u;
        }

        public List<Pago> GetPagosByUser(Usuario user)
        {
            List<Pago> pagosDeUser = new();
            foreach (Pago pago in _pagos)
            {
                if (pago.Usuario.Equals(user))
                {
                    pagosDeUser.Add(pago);
                }
            }
            return pagosDeUser;
        }

        public List<Pago> GetPagosByUserByMonth(Usuario u, DateTime fecha)
        {
            List<Pago> gastosDelMes = GetPagosDelMes(fecha);
            List<Pago> match = new List<Pago>();
            foreach (Pago p in gastosDelMes)
            {
                if (p.Usuario.Equals(u)) match.Add(p);
            }
            match.Sort();
            return match;
        }

        public string GetDominio()
        {
            return Usuario.Dominio;
        }

        public Equipo GetEquipoByName(string? nombreEquipo)
        {
            Equipo match = null;
            foreach (Equipo equipo in _equipos)
            {
                if (equipo.Nombre == nombreEquipo) match = equipo;
            }

            return match;
        }

        public TipoDeGasto FindTipoDeGastoById(int id)
        {
            Console.WriteLine(id);
            foreach (TipoDeGasto t in _tiposDeGastos)
            {
                if (t.Id.Equals(id)) return t;
            }
            throw new Exception("El tipo de gasto no existe.");
        }

        public Usuario getUserById(int? id)
        {
            foreach (Usuario user in _usuarios)
            {
                if (user.Id.Equals(id)) return user;
            }
            return null;
        }

        public List<Pago> GetPagosByTeamByMonth(int equipoId, DateTime fecha)
        {
            List<Pago> gastosDelMes = GetPagosDelMes(fecha);

            List<Pago> match = new List<Pago>();
            foreach (Pago p in gastosDelMes)
            {
                if (p.Usuario.Equipo.Id.Equals(equipoId)) match.Add(p);
            }
            match.Sort();
            return match;
        }

        public decimal GetTotalPagosByList(IEnumerable<Pago> pagos)
        {
            decimal total = 0;
            foreach (Pago p in pagos)
            {
                total += p.MontoTotal;
            }
            return total;
        }

        public decimal CalcTotalOfMonth(Usuario user, DateTime fecha)
        {
            decimal total = 0;

            List<Pago> pagosEsteMes = GetPagosByUserByMonth(user, fecha);
            foreach (Pago p in pagosEsteMes)
            {
                total += p.MontoTotal;
            }
            return (decimal)total;
        }


        public IEnumerable<(string Month, decimal Total)> GetTotalsLastMonths(
            Usuario user,
            DateTime date)
        {
            var culture = new CultureInfo("es-UY");
            var start = new DateTime(date.Year, date.Month, 1);

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = start.AddMonths(-i);

                yield return (
                    monthDate.ToString("MMM", culture).ToLower(),
                    CalcTotalOfMonth(user, monthDate)
                );
            }
        }


        public bool TipoDeGastoInUse(TipoDeGasto tipo)
        {
            foreach (Pago p in _pagos)
            {
                if (p is Subscripcion s && s.TipoGasto.Id == tipo.Id && s.IsActive)
                    return true;

                if (p is Cuotas c && c.TipoGasto.Id == tipo.Id && c.IsActive)
                    return true;
            }

            return false;
        }

        public decimal CalcTeamBudget(int id)
        {
            decimal budget = 0;
            foreach (Usuario u in _usuarios)
            {
                if (u.Id == id)
                {
                    budget += u.CalcPersonalBudget();
                }
            }

            return budget;
        }


        public BudgetStatus ValidateBudget(Pago payment)
        {
            decimal total = payment.CalcTotal();
            decimal budget = payment.Usuario.CalcPersonalBudget();
            decimal UserMonthly = CalcTotalOfMonth(payment.Usuario, DateTime.Now);
            if (total + UserMonthly > budget) return BudgetStatus.Over;
            if (total + UserMonthly > budget * 0.8m) return BudgetStatus.Close;
            return BudgetStatus.Allowed;
        }

    }
}

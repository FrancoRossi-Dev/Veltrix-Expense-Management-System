using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Domain.Usuarios.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Sistema
    {
        private Sistema()
        {
            Precarga();
        }

        static private Sistema? _sistema = null;
        static public Sistema GetSistema()
        {
            if (_sistema == null) _sistema = new Sistema();
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

        public void PrecargaRoles()
        {
            RolGerente rolGerente = new();
            RolEmpleado rolEmpleado = new();
            AltaRol(rolGerente);
            AltaRol(rolEmpleado);
        }

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

        #region Precarga

        private void Precarga()
        {
            // Precarga actualizada para Noviembre 2025
            PrecargaRoles();
            PrecargaEquipos();
            PrecargaUsuarios();
            PrecargaTiposDeGasto();
            PrecargaPagos();
        }

        private void PrecargaEquipos()
        {
            List<Equipo> PRECARGA_EQUIPOS = new()
            {
                new("Desarrollo Backend", 1000m),
                new("Desarrollo Frontend - Web", 750m),
                new("Desarrollo Frontend - Mobile", 750m),
                new("DevOps & Infrastructure", 500m),
                new("QA & Testing", 700m),
                new("UX/UI Design", 500m),
                new("Product Management", 1000m),
                new("Data Science & Analytics", 750m)
            };
            PRECARGA_EQUIPOS.ForEach(AltaEquipo);
        }

        private void PrecargaUsuarios()
        {
            List<Usuario> PRECARGA_USUARIOS = new()
            {
                new("Martín", "González", _equipos[0], "Martin2023", new DateTime(2023, 03, 15)),
                new("Sofía", "Rodríguez", _equipos[3], "Sofia123!", new DateTime(2023, 06, 20)),
                new("Lucas", "Fernández", _equipos[2], "Lucas2024", new DateTime(2024, 01, 10)),
                new("Valentina", "López", _equipos[5], "Vale1234", new DateTime(2023, 09, 05)),
                new("Diego", "Martínez", _equipos[1], "Diego456!", new DateTime(2024, 02, 14)),
                new("Camila", "Sánchez", _equipos[4], "Cami2023*", new DateTime(2023, 11, 22)),
                new("Mateo", "Pérez", _equipos[2], "Mateo789", new DateTime(2024, 03, 08)),
                new("Isabella", "García", _equipos[6], "Isa12345", new DateTime(2023, 07, 30)),
                new("Santiago", "Romero", _equipos[1], "Santi2024!", new DateTime(2024, 04, 12)),
                new("Abril", "Torres", _equipos[3], "Abril987*", new DateTime(2023, 08, 18)),
                new("Joaquín", "Álvarez", _equipos[5], "Joaqui23", new DateTime(2024, 01, 25)),
                new("Emma", "Benítez", _equipos[2], "Emma2023!", new DateTime(2023, 10, 07)),
                new("Benjamín", "Castro", _equipos[4], "Benja456", new DateTime(2024, 02, 28)),
                new("Mía", "Méndez", _equipos[7], "Mia12345*", new DateTime(2023, 05, 16)),
                new("Thiago", "Vargas", _equipos[1], "Thiago24!", new DateTime(2024, 03, 20)),
                new("Catalina", "Silva", _equipos[6], "Cata2023", new DateTime(2023, 12, 11)),
                new("Marcos", "González", _equipos[3], "Marc2024!", new DateTime(2024, 01, 09)),
                new("Renata", "Morales", _equipos[2], "Rena2023*", new DateTime(2023, 04, 23)),
                new("Felipe", "Ortiz", _equipos[5], "Felipe99", new DateTime(2024, 02, 15)),
                new("Lucía", "Ramos", _equipos[7], "Lucia456!", new DateTime(2023, 09, 28)),
                new("Marina", "González", _equipos[4], "Mari2024*", new DateTime(2024, 03, 05)),
                new("Olivia", "Domínguez", _equipos[6], "Oli12345", new DateTime(2023, 11, 14))
            };
            PRECARGA_USUARIOS.ForEach(AltaUsuario);

            foreach (Usuario u in _usuarios)
            {
                // los primeros 5 usuarios tendran rol de gerente
                if (u.Id <= 5)
                {
                    u.AddRole(_roles[0]);
                }
                u.AddRole(_roles[1]);

            }
        }

        private void PrecargaTiposDeGasto()
        {
            List<TipoDeGasto> PRECARGA_TIPOS_DE_GASTO = new()
            {
                new("Software & SaaS", "Suscripciones a herramientas y servicios en la nube"),
                new("Hardware & Equipamiento", "Computadoras, periféricos y equipamiento tecnológico"),
                new("Infraestructura Cloud", "Servicios de hosting, servidores y almacenamiento"),
                new("Capacitación", "Cursos, certificaciones y desarrollo profesional"),
                new("Transporte", "Viáticos, combustible y transporte para el equipo"),
                new("Eventos & Networking", "Conferencias, meetups y eventos de la industria"),
                new("Team Building", "Actividades recreativas y afters del equipo"),
                new("Oficina & Suministros", "Materiales de oficina y suministros generales"),
                new("Marketing & Publicidad", "Campañas, publicidad digital y promociones"),
                new("Servicios Profesionales", "Consultorías, asesorías legales y contables")
            };
            PRECARGA_TIPOS_DE_GASTO.ForEach(AltaTipoDeGasto);
        }

        private void PrecargaPagos()
        {
            // 15 Pagos recurrentes activos (suscripciones sin fecha fin) - muchos iniciados en noviembre 2025
            List<Subscripcion> PRECARGA_SUBSCRIPCIONES = new()
                    {
                        new(299, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[1], "GitHub Enterprise", new DateTime(2025, 11, 01)),
                        new(450, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[3], "AWS Cloud Services", new DateTime(2025, 11, 05)),
                        new(89, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[5], "Figma Professional", new DateTime(2025, 11, 10)),
                        new(199, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[7], "Jira Software Premium", new DateTime(2025, 11, 15)),
                        new(75, MetodoDePago.DEBITO, _tiposDeGastos[0], _usuarios[2], "Slack Business+", new DateTime(2025, 11, 08)),
                        new(350, MetodoDePago.CREDITO, _tiposDeGastos[2], _usuarios[4], "Azure DevOps Services", new DateTime(2025, 11, 12)),
                        new(120, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[0], "Notion Team Plan", new DateTime(2025, 11, 03)),
                        new(189, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[8], "Adobe Creative Cloud", new DateTime(2025, 10, 20)),
                        new(95, MetodoDePago.DEBITO, _tiposDeGastos[0], _usuarios[10], "Postman Enterprise", new DateTime(2025, 10, 15)),
                        new(249, MetodoDePago.CREDITO, _tiposDeGastos[2], _usuarios[12], "MongoDB Atlas", new DateTime(2025, 09, 30)),
                        new(159, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[0], "Zoom Business", new DateTime(2025, 11, 18)),
                        new(299, MetodoDePago.CREDITO, _tiposDeGastos[2], _usuarios[16], "Google Cloud Platform", new DateTime(2025, 11, 22)),
                        new(79, MetodoDePago.DEBITO, _tiposDeGastos[0], _usuarios[18], "Miro Team", new DateTime(2025, 11, 07)),
                        new(199, MetodoDePago.CREDITO, _tiposDeGastos[0], _usuarios[11], "Confluence Enterprise", new DateTime(2025, 10, 10)),
                        new(449, MetodoDePago.CREDITO, _tiposDeGastos[2], _usuarios[0], "Vercel Pro Team", new DateTime(2025, 11, 20))
                    };
            PRECARGA_SUBSCRIPCIONES.ForEach(AltaPago);

            // 10 Pagos en cuotas (5 finalizados, 5 incompletos)
            List<Cuotas> PRECARGA_CUOTAS = new()
                    {
                        // 5 Cuotas finalizadas (fecha fin pasada - finalizadas antes de nov 2025)
                        new(275, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[8], "Dell XPS 15 - 8 cuotas", new DateTime(2024, 05, 15), new DateTime(2024, 12, 15)),
                        new(148, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[14], "Monitor LG UltraWide - 8 cuotas", new DateTime(2024, 06, 20), new DateTime(2025, 01, 20)),
                        new(200, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[9], "Certificación Google Cloud - 7 cuotas", new DateTime(2024, 07, 10), new DateTime(2025, 02, 10)),
                        new(200, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[15], "iMac 24 pulgadas - 14 cuotas", new DateTime(2024, 01, 20), new DateTime(2025, 02, 20)),
                        new(150, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[20], "Silla ergonómica Herman Miller - 7 cuotas", new DateTime(2024, 12, 15), new DateTime(2025, 06, 15)),
                        // 5 Cuotas incompletas (fecha fin en futuro - 2026/2027)
                        new(118, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[1], "MacBook Pro M4 - 13 cuotas", new DateTime(2025, 11, 10), new DateTime(2026, 11, 10)),
                        new(177, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[5], "Curso AWS Solutions Architect - 7 cuotas", new DateTime(2025, 11, 05), new DateTime(2026, 05, 05)),
                        new(105, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[19], "Laptop Lenovo ThinkPad - 13 cuotas", new DateTime(2025, 09, 25), new DateTime(2026, 09, 25)),
                        new(150, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[11], "Workstation Dell Precision - 19 cuotas", new DateTime(2025, 05, 01), new DateTime(2026, 11, 01)),
                        new(100, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[0], "Máster en Data Science - 13 cuotas", new DateTime(2025, 10, 01), new DateTime(2026, 10, 01))
                    };
            PRECARGA_CUOTAS.ForEach(AltaPago);

            // 20 Pagos únicos - muchos en noviembre 2025
            List<Unico> PRECARGA_PAGOS_UNICOS = new()
                {
                    // Pagos de noviembre 2025
                    new(4800, MetodoDePago.CREDITO, _tiposDeGastos[5], _usuarios[3], "Conferencia AWS re:Invent - Las Vegas", new DateTime(2025, 11, 02), "REC-2025-001578"),
                    new(195, MetodoDePago.EFECTIVO, _tiposDeGastos[6], _usuarios[7], "After office equipo Backend", new DateTime(2025, 11, 04), "REC-2025-002341"),
                    new(280, MetodoDePago.DEBITO, _tiposDeGastos[4], _usuarios[2], "Uber para visita cliente", new DateTime(2025, 11, 06), "REC-2025-003156"),
                    new(920, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[11], "Certificación Scrum Master", new DateTime(2025, 11, 08), "REC-2025-000892"),
                    new(350, MetodoDePago.EFECTIVO, _tiposDeGastos[6], _usuarios[6], "Cena team building QA", new DateTime(2025, 11, 11), "REC-2025-004267"),
                    new(1350, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[15], "Teclado mecánico Keychron Q6", new DateTime(2025, 11, 13), "REC-2025-000543"),
                    new(105, MetodoDePago.DEBITO, _tiposDeGastos[4], _usuarios[9], "Taxi aeropuerto - reunión clientes", new DateTime(2025, 11, 16), "REC-2025-004789"),
                    new(3200, MetodoDePago.CREDITO, _tiposDeGastos[5], _usuarios[4], "Google Cloud Next", new DateTime(2025, 11, 18), "REC-2025-001823"),
                    new(480, MetodoDePago.EFECTIVO, _tiposDeGastos[6], _usuarios[12], "Actividad escape room equipo", new DateTime(2025, 11, 19), "REC-2025-003654"),
                    new(750, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[18], "Curso Figma Advanced", new DateTime(2025, 11, 21), "REC-2025-002198"),
                    new(165, MetodoDePago.DEBITO, _tiposDeGastos[7], _usuarios[10], "Resma papel y útiles oficina", new DateTime(2025, 11, 23), "REC-2025-004102"),
                    new(3800, MetodoDePago.CREDITO, _tiposDeGastos[5], _usuarios[16], "React Advanced London", new DateTime(2025, 11, 24), "REC-2025-002176"),
                    new(240, MetodoDePago.EFECTIVO, _tiposDeGastos[6], _usuarios[13], "Pizza party release v3.0", new DateTime(2025, 11, 23), "REC-2025-005023"),
                    new(1580, MetodoDePago.CREDITO, _tiposDeGastos[1], _usuarios[20], "Mouse Logitech MX Master 3S", new DateTime(2025, 11, 25), "REC-2025-001234"),
                    new(420, MetodoDePago.DEBITO, _tiposDeGastos[4], _usuarios[17], "Combustible viaje presentación", new DateTime(2025, 11, 25), "REC-2025-004512"),
                    // Pagos de meses anteriores (2025)
                    new(105, MetodoDePago.EFECTIVO, _tiposDeGastos[4], _usuarios[21], "Estacionamiento evento networking", new DateTime(2025, 10, 07), "REC-2025-004834"),
                    new(620, MetodoDePago.CREDITO, _tiposDeGastos[3], _usuarios[14], "Workshop Docker y Kubernetes", new DateTime(2025, 10, 16), "REC-2025-003421"),
                    new(520, MetodoDePago.CREDITO, _tiposDeGastos[8], _usuarios[5], "Campaña Google Ads Q4", new DateTime(2025, 10, 01), "REC-2025-005234"),
                    new(980, MetodoDePago.CREDITO, _tiposDeGastos[9], _usuarios[0], "Consultoría legal contratos", new DateTime(2025, 09, 15), "REC-2025-004567"),
                    new(295, MetodoDePago.DEBITO, _tiposDeGastos[7], _usuarios[19], "Café y snacks para oficina", new DateTime(2025, 10, 20), "REC-2025-005678")
                };
            PRECARGA_PAGOS_UNICOS.ForEach(AltaPago);
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
            foreach(Usuario u in _usuarios)
            {
                if (u.Id == id)
                {
                budget += u.CalcPersonalBudget();
                }
            }

            return budget;
        }
    }
}

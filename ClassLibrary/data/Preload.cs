using Domain.Pagos;
using Domain.Pagos.tipos;
using Domain.Usuarios;
using Domain.Usuarios.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.data
{
    internal class Preload
    {
        public Sistema Sys { get; set; } = Sistema.GetSistema();
        public Preload() {
        }
        
        public void Precarga()
        {
            // Precarga actualizada para Noviembre 2025
            PrecargaRoles();
            PrecargaEquipos();
            PrecargaUsuarios();
            PrecargaTiposDeGasto();
            PrecargaPagos();
        }

        public void PrecargaRoles()
        {
            RolGerente rolGerente = new();
            RolEmpleado rolEmpleado = new();
            Sys.AltaRol(rolGerente);
            Sys.AltaRol(rolEmpleado);

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
            PRECARGA_EQUIPOS.ForEach(Sys.AltaEquipo);
        }

        private void PrecargaUsuarios()
        {
            List<Equipo> equipos = Sys.GetEquipos();

            List<Usuario> PRECARGA_USUARIOS = new()
            {
                new("Martín", "González", equipos[0], "Martin2023", new DateTime(2023, 03, 15)),
                new("Sofía", "Rodríguez", equipos[3], "Sofia123!", new DateTime(2023, 06, 20)),
                new("Lucas", "Fernández", equipos[2], "Lucas2024", new DateTime(2024, 01, 10)),
                new("Valentina", "López", equipos[5], "Vale1234", new DateTime(2023, 09, 05)),
                new("Diego", "Martínez", equipos[1], "Diego456!", new DateTime(2024, 02, 14)),
                new("Camila", "Sánchez", equipos[4], "Cami2023*", new DateTime(2023, 11, 22)),
                new("Mateo", "Pérez", equipos[2], "Mateo789", new DateTime(2024, 03, 08)),
                new("Isabella", "García", equipos[6], "Isa12345", new DateTime(2023, 07, 30)),
                new("Santiago", "Romero", equipos[1], "Santi2024!", new DateTime(2024, 04, 12)),
                new("Abril", "Torres", equipos[3], "Abril987*", new DateTime(2023, 08, 18)),
                new("Joaquín", "Álvarez", equipos[5], "Joaqui23", new DateTime(2024, 01, 25)),
                new("Emma", "Benítez", equipos[2], "Emma2023!", new DateTime(2023, 10, 07)),
                new("Benjamín", "Castro", equipos[4], "Benja456", new DateTime(2024, 02, 28)),
                new("Mía", "Méndez", equipos[7], "Mia12345*", new DateTime(2023, 05, 16)),
                new("Thiago", "Vargas", equipos[1], "Thiago24!", new DateTime(2024, 03, 20)),
                new("Catalina", "Silva", equipos[6], "Cata2023", new DateTime(2023, 12, 11)),
                new("Marcos", "González", equipos[3], "Marc2024!", new DateTime(2024, 01, 09)),
                new("Renata", "Morales", equipos[2], "Rena2023*", new DateTime(2023, 04, 23)),
                new("Felipe", "Ortiz", equipos[5], "Felipe99", new DateTime(2024, 02, 15)),
                new("Lucía", "Ramos", equipos[7], "Lucia456!", new DateTime(2023, 09, 28)),
                new("Marina", "González", equipos[4], "Mari2024*", new DateTime(2024, 03, 05)),
                new("Olivia", "Domínguez", equipos[6], "Oli12345", new DateTime(2023, 11, 14))
            };
            PRECARGA_USUARIOS.ForEach(Sys.AltaUsuario);

            List<Usuario> usuarios = Sys.GetUsuarios();
            List<Rol> roles = Sys.GetRoles();

            foreach (Usuario u in usuarios)
            {
                // los primeros 5 usuarios tendran rol de gerente
                if (u.Id <= 5)
                {
                    u.AddRole(roles[0]);
                }
                u.AddRole(roles[1]);

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
            PRECARGA_TIPOS_DE_GASTO.ForEach(Sys.AltaTipoDeGasto);
        }

        private void PrecargaPagos()
        {
            List<TipoDeGasto> tiposDeGastos = Sys.GetTipoDeGastos();
            List<Usuario> usuarios = Sys.GetUsuarios();
            // 15 Pagos recurrentes activos (suscripciones sin fecha fin) - muchos iniciados en noviembre 2025
            List<Subscripcion> PRECARGA_SUBSCRIPCIONES = new()
                    {
                        new(299, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[1], "GitHub Enterprise", new DateTime(2025, 11, 01)),
                        new(450, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[3], "AWS Cloud Services", new DateTime(2025, 11, 05)),
                        new(89, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[5], "Figma Professional", new DateTime(2025, 11, 10)),
                        new(199, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[7], "Jira Software Premium", new DateTime(2025, 11, 15)),
                        new(75, MetodoDePago.DEBITO, tiposDeGastos[0], usuarios[2], "Slack Business+", new DateTime(2025, 11, 08)),
                        new(350, MetodoDePago.CREDITO, tiposDeGastos[2], usuarios[4], "Azure DevOps Services", new DateTime(2025, 11, 12)),
                        new(120, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[0], "Notion Team Plan", new DateTime(2025, 11, 03)),
                        new(189, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[8], "Adobe Creative Cloud", new DateTime(2025, 10, 20)),
                        new(95, MetodoDePago.DEBITO, tiposDeGastos[0], usuarios[10], "Postman Enterprise", new DateTime(2025, 10, 15)),
                        new(249, MetodoDePago.CREDITO, tiposDeGastos[2], usuarios[12], "MongoDB Atlas", new DateTime(2025, 09, 30)),
                        new(159, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[0], "Zoom Business", new DateTime(2025, 11, 18)),
                        new(299, MetodoDePago.CREDITO, tiposDeGastos[2], usuarios[16], "Google Cloud Platform", new DateTime(2025, 11, 22)),
                        new(79, MetodoDePago.DEBITO, tiposDeGastos[0], usuarios[18], "Miro Team", new DateTime(2025, 11, 07)),
                        new(199, MetodoDePago.CREDITO, tiposDeGastos[0], usuarios[11], "Confluence Enterprise", new DateTime(2025, 10, 10)),
                        new(449, MetodoDePago.CREDITO, tiposDeGastos[2], usuarios[0], "Vercel Pro Team", new DateTime(2025, 11, 20))
                    };
            PRECARGA_SUBSCRIPCIONES.ForEach(Sys.AltaPago);

            // 10 Pagos en cuotas (5 finalizados, 5 incompletos)
            List<Cuotas> PRECARGA_CUOTAS = new()
                    {
                        // 5 Cuotas finalizadas (fecha fin pasada - finalizadas antes de nov 2025)
                        new(275, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[8], "Dell XPS 15 - 8 cuotas", new DateTime(2024, 05, 15), new DateTime(2024, 12, 15)),
                        new(148, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[14], "Monitor LG UltraWide - 8 cuotas", new DateTime(2024, 06, 20), new DateTime(2025, 01, 20)),
                        new(200, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[9], "Certificación Google Cloud - 7 cuotas", new DateTime(2024, 07, 10), new DateTime(2025, 02, 10)),
                        new(200, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[15], "iMac 24 pulgadas - 14 cuotas", new DateTime(2024, 01, 20), new DateTime(2025, 02, 20)),
                        new(150, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[20], "Silla ergonómica Herman Miller - 7 cuotas", new DateTime(2024, 12, 15), new DateTime(2025, 06, 15)),
                        // 5 Cuotas incompletas (fecha fin en futuro - 2026/2027)
                        new(118, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[1], "MacBook Pro M4 - 13 cuotas", new DateTime(2025, 11, 10), new DateTime(2026, 11, 10)),
                        new(177, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[5], "Curso AWS Solutions Architect - 7 cuotas", new DateTime(2025, 11, 05), new DateTime(2026, 05, 05)),
                        new(105, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[19], "Laptop Lenovo ThinkPad - 13 cuotas", new DateTime(2025, 09, 25), new DateTime(2026, 09, 25)),
                        new(150, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[11], "Workstation Dell Precision - 19 cuotas", new DateTime(2025, 05, 01), new DateTime(2026, 11, 01)),
                        new(100, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[0], "Máster en Data Science - 13 cuotas", new DateTime(2025, 10, 01), new DateTime(2026, 10, 01))
                    };
            PRECARGA_CUOTAS.ForEach(Sys.AltaPago);

            // 20 Pagos únicos - muchos en noviembre 2025
            List<Unico> PRECARGA_PAGOS_UNICOS = new()
                {
                    // Pagos de noviembre 2025
                    new(4800, MetodoDePago.CREDITO, tiposDeGastos[5], usuarios[3], "Conferencia AWS re:Invent - Las Vegas", new DateTime(2025, 11, 02), "REC-2025-001578"),
                    new(195, MetodoDePago.EFECTIVO, tiposDeGastos[6], usuarios[7], "After office equipo Backend", new DateTime(2025, 11, 04), "REC-2025-002341"),
                    new(280, MetodoDePago.DEBITO, tiposDeGastos[4], usuarios[2], "Uber para visita cliente", new DateTime(2025, 11, 06), "REC-2025-003156"),
                    new(920, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[11], "Certificación Scrum Master", new DateTime(2025, 11, 08), "REC-2025-000892"),
                    new(350, MetodoDePago.EFECTIVO, tiposDeGastos[6], usuarios[6], "Cena team building QA", new DateTime(2025, 11, 11), "REC-2025-004267"),
                    new(1350, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[15], "Teclado mecánico Keychron Q6", new DateTime(2025, 11, 13), "REC-2025-000543"),
                    new(105, MetodoDePago.DEBITO, tiposDeGastos[4], usuarios[9], "Taxi aeropuerto - reunión clientes", new DateTime(2025, 11, 16), "REC-2025-004789"),
                    new(3200, MetodoDePago.CREDITO, tiposDeGastos[5], usuarios[4], "Google Cloud Next", new DateTime(2025, 11, 18), "REC-2025-001823"),
                    new(480, MetodoDePago.EFECTIVO, tiposDeGastos[6], usuarios[12], "Actividad escape room equipo", new DateTime(2025, 11, 19), "REC-2025-003654"),
                    new(750, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[18], "Curso Figma Advanced", new DateTime(2025, 11, 21), "REC-2025-002198"),
                    new(165, MetodoDePago.DEBITO, tiposDeGastos[7], usuarios[10], "Resma papel y útiles oficina", new DateTime(2025, 11, 23), "REC-2025-004102"),
                    new(3800, MetodoDePago.CREDITO, tiposDeGastos[5], usuarios[16], "React Advanced London", new DateTime(2025, 11, 24), "REC-2025-002176"),
                    new(240, MetodoDePago.EFECTIVO, tiposDeGastos[6], usuarios[13], "Pizza party release v3.0", new DateTime(2025, 11, 23), "REC-2025-005023"),
                    new(1580, MetodoDePago.CREDITO, tiposDeGastos[1], usuarios[20], "Mouse Logitech MX Master 3S", new DateTime(2025, 11, 25), "REC-2025-001234"),
                    new(420, MetodoDePago.DEBITO, tiposDeGastos[4], usuarios[17], "Combustible viaje presentación", new DateTime(2025, 11, 25), "REC-2025-004512"),
                    // Pagos de meses anteriores (2025)
                    new(105, MetodoDePago.EFECTIVO, tiposDeGastos[4], usuarios[21], "Estacionamiento evento networking", new DateTime(2025, 10, 07), "REC-2025-004834"),
                    new(620, MetodoDePago.CREDITO, tiposDeGastos[3], usuarios[14], "Workshop Docker y Kubernetes", new DateTime(2025, 10, 16), "REC-2025-003421"),
                    new(520, MetodoDePago.CREDITO, tiposDeGastos[8], usuarios[5], "Campaña Google Ads Q4", new DateTime(2025, 10, 01), "REC-2025-005234"),
                    new(980, MetodoDePago.CREDITO, tiposDeGastos[9], usuarios[0], "Consultoría legal contratos", new DateTime(2025, 09, 15), "REC-2025-004567"),
                    new(295, MetodoDePago.DEBITO, tiposDeGastos[7], usuarios[19], "Café y snacks para oficina", new DateTime(2025, 10, 20), "REC-2025-005678")
                };
            PRECARGA_PAGOS_UNICOS.ForEach(Sys.AltaPago);
        }
    }
}

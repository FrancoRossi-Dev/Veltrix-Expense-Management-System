using Domain.Usuarios.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Usuarios
{
    public class Usuario : IValidable, IComparable<Usuario>
    {
        
        static public int IdCount { get; set; } = 1;
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public Equipo Equipo { get; set; }
        public string Contrasenia { get; set; }
        public DateTime FechaIncorporacion { get; set; }
        public string Email { get; set; }

        private List<Rol> _rol = new List<Rol>(); 
        public void AddRole(Rol r)
        {
            _rol.Add(r);
        }
        public void RemoveRol(Rol r)
        {
            _rol.Remove(r);
        }
        public List<Rol> GetRoles()
        {
            return _rol;
        }

        public string MiPuesto()
        {
            string misRoles = "";
            foreach(Rol r in _rol)
            {
                misRoles += r.Titulo + " ";
            }
            if (misRoles.Contains("Gerente")) return "Gerente";
            if (misRoles.Contains("Empleado")) return "Empleado";

            return "Desconocido";
        }

        public static string Dominio { get; set; } = "@laEmpresa.com";
        
        public Usuario()
        {
            Id = IdCount++;
        }

        public Usuario(string nombre, string apellido, Equipo equipo, string contrasenia, DateTime fechaIncorporacion)
        {
            Id = IdCount++;
            Nombre = CapitalizeNames(nombre);
            Apellido = CapitalizeNames(apellido);
            Equipo = equipo;
            Contrasenia = contrasenia;
            FechaIncorporacion = fechaIncorporacion;
            Validate();
        }

        private string CapitalizeNames(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            texto = texto.ToLower();
            return char.ToUpper(texto[0]) + texto.Substring(1);
        }
        
        public void GenerateEmail(List<Usuario> usuarios)
        {
            // primero conseguimos el texto con el nombre y apellido de maximo 3 chars cada uno
            string firstPart = TextoDeTresLetras(Nombre) + TextoDeTresLetras(Apellido);
            // buscamos el numero que corresponde para el nuevo mail
            int numero = NumeroParaNuevoEmail(firstPart, usuarios);
            if (numero > 0)
            {
                Email = firstPart + numero + Dominio;
            }
            else
            {
                Email = firstPart + Dominio;
            }
        }
        
        private int NumeroParaNuevoEmail(string email, List<Usuario> usuarios)
        {
            for (int i = usuarios.Count - 1; i >= 0 ; i--)
            {
                if (usuarios[i].Email.Substring(0, email.Length) == email)
                {
                    int index = usuarios[i].Email.IndexOf("@");
                    int largo = index - email.Length;
                    if (largo == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        int numero = int.Parse(usuarios[i].Email.Substring(email.Length, largo));
                        return numero + 1;
                    }
                }
            }
            return 0;
        }


        private string TextoDeTresLetras(string texto)
        {
            texto = EliminarTildes(texto);
            if (texto.Length > 3)
            {
                return texto.Substring(0, 3);
            }
            
            return texto;
        }
        
        public string EliminarTildes(string texto) {
            texto = texto.Replace("á", "a").Replace("Á", "A");
            texto = texto.Replace("é", "e").Replace("É", "E");
            texto = texto.Replace("í", "i").Replace("Í", "I");
            texto = texto.Replace("ó", "o").Replace("Ó", "O");
            texto = texto.Replace("ú", "u").Replace("Ú", "U");
            texto = texto.Replace("ü", "u").Replace("Ü", "U");
            return texto;
        }



        public override bool Equals(object? obj)
        {
            return obj is Usuario usuario &&
                   Email.ToLower() == usuario.Email.ToLower();
        }

        #region Validation

        public void Validate()
        {
            ValidateNombre();
            ValidateApellido();
            ValidateContrasenia();
            ValidateFecha();
        }

        private void ValidateFecha()
        {
            if (FechaIncorporacion == default) throw new Exception("Debe ingresar la fecha de incorporacion");
        }

        private void ValidateContrasenia()
        {
            if (String.IsNullOrEmpty(Contrasenia) || Contrasenia.Length < 8) 
                throw new Exception("La contraseña debe tener un minimo de 8 caracteres");
        }

        private void ValidateApellido()
        {
            if (String.IsNullOrEmpty(Apellido)) 
                throw new Exception("Apellido no puede ser vacio");
        }

        private void ValidateNombre()
        {
            if(String.IsNullOrEmpty(Nombre))
                throw new Exception("Nombre no puede ser vacio");

        }
        #endregion

        public bool PertenceA(string equipoNombre)
        {
            return equipoNombre == Equipo.Nombre;
        }

        public string QuienSoy(bool includeTeam)
        {
            return $"{Nombre} {Apellido}, email: {Email}, {(includeTeam ? $"miembro de {Equipo}" : string.Empty)}";
        }


        public int CompareTo(Usuario? other)
        {
            if (other == null) return 0;
            return Email.CompareTo(other.Email);
        }
    }
}

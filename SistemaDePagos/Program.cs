using Domain;
using Domain.Usuarios;
using Domain.Pagos;

namespace SistemaDePagos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool isRunning = true;
            Console.WriteLine("Bienvenido!");
            Sistema? sis = null;

            try
            {
                sis = Sistema.GetSistema();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("No se pudo inicializar el sistema");
                Console.WriteLine(Ex.Message);
                isRunning = false;
            }

            #region menu ui

            while (isRunning)
            {
                Console.WriteLine("elija una opción:");
                Console.WriteLine("1. Listar todos los usuarios en sistema");
                Console.WriteLine("2. Listar todos los pagos de un usuario");
                Console.WriteLine("3. Dar de alta a un nuevo usuario");
                Console.WriteLine("4. Listar todos los miembros de un equipo");
                Console.WriteLine("0. Para cerrar el programa");
                bool isSelecting = true;
                while (isSelecting)
                {
                    try
                    {
                        int selection = int.Parse(Console.ReadLine());
                        
                        switch (selection)
                        {
                            case 0:
                                isRunning = false;
                                break;
                            case 1:
                                PrintAllUsers(sis.GetUsuarios(), includeTeam: true);
                                break;
                            case 2:
                                PrintPayments();
                                break;
                            case 3:
                                RegistrarUsuario();
                                break;
                            case 4:
                                PrintMembersByTeam();
                                break;
                            default:
                                throw new Exception("opcion invalida, porfavor selecione una opcion valida");
                        }
                        isSelecting = false;

                        if (selection != 0)
                        {
                            Console.WriteLine("Presione cualquier tecla para regresar al menu");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ingrese un numero que corresponda a una de las opciones.");
                            Console.ResetColor();
                        }

                        else Console.WriteLine(ex.Message);
                    }
                }

            }
            Console.WriteLine("Ejecución finalizada, presione cualquier tecla para salir");
            Console.ReadKey();
            #endregion

        }

        private static void PrintMembersByTeam()
        {
            Sistema sis = Sistema.GetSistema();
            Equipo equipo = SelectEquipo();
            List<Usuario> usuarios = sis.GetUsuariosPorEquipo(equipo.Nombre);
            PrintAllUsers(usuarios, includeTeam: false);
        }

        private static void RegistrarUsuario()
        {
            Sistema sis = Sistema.GetSistema();
            Console.WriteLine("Creando un nuevo usuario...");
            try
            {
                string nombre = ValidateInput("Ingrese el nombre de usuario");

                string apellido = ValidateInput("Ingrese el apellido");

                Console.WriteLine("a que equipo pertence");
                Equipo equipo = SelectEquipo();

                string contrasenia = ValidateInput("asigne una contraseña");

                int anio = int.Parse(ValidateInput("ingrese el año de ingreso"));

                int mes = int.Parse(ValidateInput("ingrese el mes de ingreso"));

                int dia = int.Parse(ValidateInput("ingrese el dia de ingreso"));

                DateTime fechaIngreso = new DateTime(anio, mes, dia);

                sis.AltaUsuario(new Usuario(nombre, apellido, equipo, contrasenia, fechaIngreso));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Usuario {nombre} {apellido} registrado con exito");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string ValidateInput(string title)
        {
            bool inputCorrect = false;
            string inputValue = "";
            Console.WriteLine(title);
            while (!inputCorrect)
            {
                inputValue = Console.ReadLine();
                if (inputValue.Any(char.IsWhiteSpace) || string.IsNullOrEmpty(inputValue))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("El valor ingresado no puede contener espacios en blanco");
                    Console.ResetColor();
                }
                else
                {
                    inputCorrect = true;
                }

            }
            return inputValue;
        }
        
        private static string ValidateEmail(string title)
        {
            Sistema sis = Sistema.GetSistema();
            bool inputCorrect = false;
            string inputValue = "";
            Console.WriteLine(title);
            while (!inputCorrect)
            {
                
                inputValue = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Red;
                if (inputValue.Any(char.IsWhiteSpace)|| string.IsNullOrEmpty(inputValue))
                {
                    Console.WriteLine("El valor ingresado no puede contener espacios en blanco");
                } 
                else if (!inputValue.ToLower().Contains(sis.GetDominio().ToLower()))
                {
                    Console.WriteLine("El dominio del email es incorrecto");
                }
                else
                {
                    inputCorrect = true;
                }
                Console.ResetColor();
            }
            return inputValue;
        }

        private static Equipo SelectEquipo()
        {
            Sistema sis = Sistema.GetSistema();
            List<Equipo> equiposSistema = sis.GetEquipos();
            for (int i = 0; i < equiposSistema.Count; i++)
            {
                Console.WriteLine($"{i + 1}: \"{equiposSistema[i]}\".");
            }

            Equipo equipo = null;
            bool isSelectingTeam = true;
            while (isSelectingTeam)
            {
                try
                {
                    int indexEq = int.Parse(Console.ReadLine());
                    if (indexEq <= 0 || indexEq > equiposSistema.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("El equipo no existe, seleccione otro");
                        Console.ResetColor();
                    }
                    else
                    {
                        equipo = sis.GetEquipoByName(equiposSistema[indexEq - 1].Nombre);
                        isSelectingTeam = false;
                    }
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Selecciona una de las opciones del menu...");
                    Console.ResetColor();
                }
            }

            return equipo;
        }

        private static void PrintPayments()
        {
            Sistema sis = Sistema.GetSistema();
            try
            {
                string inputMail = ValidateEmail("Ingrese el correo del usuario que desea buscar");

                Usuario user = sis.FindUserByMail(inputMail);
                if (user == null)
                    throw new Exception($"El usuario con el correo {inputMail} no existe en el sistema");


                List<Pago> payments = sis.GetPagosByUser(user);
                if (payments.Count == 0)
                    Console.WriteLine($"El usuario con el correo {inputMail} no tiene pagos a su nombre");

                foreach (Pago pay in payments)
                {
                    Console.WriteLine($"- {pay}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void PrintAllUsers(List<Usuario> usuarios, bool includeTeam)
        {
            for (int i = 0; i < usuarios.Count; i++)
            {
                Console.WriteLine($"- {usuarios[i].QuienSoy(includeTeam)}");
            }
            Console.WriteLine();
        }
    }
}


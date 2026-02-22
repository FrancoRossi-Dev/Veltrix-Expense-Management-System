namespace webApp.DTO
{
    public class UsuarioDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public EquipoDto? Equipo { get; set; }
        public string? Puesto { get; internal set; }
        public string? FechaDeIncorporacion { get; internal set; }
    }
}

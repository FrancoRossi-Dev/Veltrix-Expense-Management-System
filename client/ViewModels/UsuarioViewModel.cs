using webApp.DTO;

namespace webApp.ViewModels
{
    public class UsuarioViewModel
    {
        public UsuarioDto Usuario { get;  set; }
        public decimal TotalThisMonth { get; set; }
        public IEnumerable<UsuarioDto>? UsuariosDelEquipo { get; set; } 
    }
}

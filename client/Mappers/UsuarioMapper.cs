using Domain.Usuarios;
using webApp.DTO;

namespace webApp.Mappers
{
    public class UsuarioMapper
    {
        public static UsuarioDto ToDto(Usuario u)
        {
            EquipoDto EDto = EquipoMapper.ToDto(u.Equipo);
            return new UsuarioDto
            {
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Equipo = EDto,
                Puesto = u.MiPuesto(),
                FechaDeIncorporacion = u.FechaIncorporacion.ToShortDateString()
            };
        }
    }
}

using Domain.Usuarios;
using webApp.DTO;

namespace webApp.Mappers
{
    public class EquipoMapper
    {
        static public EquipoDto ToDto(Equipo e)
        {
            return new EquipoDto
            {
                Nombre = e.Nombre,

            };
        }
    }
}

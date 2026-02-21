using Domain.Pagos;
using webApp.DTO;

public static class PagoMapper
{
    public static PagoDto ToDto(Pago p)
    {
        return new PagoDto
        {
            Usuario = new UsuarioDto
            {
                Nombre = p.Usuario.Nombre,
                Email = p.Usuario.Email,
            },
            FormaDePago = p.Metodo.ToString(),
            Descripcion = p.Descripcion,
            FechaDePago = p.MiFechaDePago(),
            TipoDeGasto = p.TipoGasto.Nombre,
            MontoTotal = p.MontoTotal,
            TipoDePago = p.TipoDePago(),
        }; 
    }
}
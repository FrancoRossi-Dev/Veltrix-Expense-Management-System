namespace webApp.DTO
{
    public class PagoDto
    {
        public UsuarioDto Usuario { get; set; }
        public string FormaDePago { get; set; }

        public string Descripcion { get; set; }

        public string FechaDePago { get; set; }

        public string TipoDeGasto { get; set; }

        public decimal MontoTotal { get; set; }

        public string TipoDePago { get; set; }

    }

}

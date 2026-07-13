namespace RozzCaps.DTOs.Resquest
{
    public class CrearGorraRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int Categoria { get; set; }
        public List<GorraVariacionesRequestDto> Variaciones { get; set; } = new();
    }
}

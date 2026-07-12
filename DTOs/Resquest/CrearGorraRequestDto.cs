namespace RozzCaps.DTOs.Resquest
{
    public class CrearGorraRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CategoriaId { get; set; }
        public List<GorraVariacionesRequestDto> Variaciones { get; set; } = new();
    }
}

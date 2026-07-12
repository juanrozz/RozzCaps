namespace RozzCaps.DTOs.Resquest
{
    public class GorraVariacionesRequestDto
    {
        public int ColorId { get; set; }
        public int Stock { get; set; }
        public string SKU { get; set; } = string.Empty;
        public List<GorrasImagenesRequestDto> Imagenes { get; set; } = new();
    }
}

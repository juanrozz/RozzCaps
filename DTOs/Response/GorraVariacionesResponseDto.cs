using RozzCaps.DTOs.Resquest;

namespace RozzCaps.DTOs.Response
{
    public class GorraVariacionesResponseDto
    {
        public int Color { get; set; }
        public int Stock { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public List<GorrasImagenesResponseDto> Imagenes { get; set; } = new();
    }
}

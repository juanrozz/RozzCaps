using RozzCaps.DTOs.Resquest;

namespace RozzCaps.DTOs.Response
{
    public class CrearGorraResponseDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int Categoria { get; set; }
        public bool Activo { get; set; }
        public List<GorraVariacionesResponseDto> Variaciones { get; set; } = new();
    }
}

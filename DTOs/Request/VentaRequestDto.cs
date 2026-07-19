using RozzCaps.DTOs.Request;

namespace RozzCaps.DTOs.Resquest
{
    public class VentaRequestDto
    {
        public List<ItemCarritoRequestDto> Items { get; set; } = new List<ItemCarritoRequestDto>();
        public EnvioRequestDto DatosEnvio { get; set; } = new EnvioRequestDto();
    }
}

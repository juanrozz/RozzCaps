namespace RozzCaps.DTOs.Response
{
    public class VentaResponseDto
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}

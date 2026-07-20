using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class Venta
{
    public int Id { get; set; }

    public string CodigoOrden { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public string? PasarelaPagoId { get; set; }

    public decimal CostoEnvio { get; set; }

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();

    public virtual ICollection<VentaEnvio> VentaEnvios { get; set; } = new List<VentaEnvio>();
}

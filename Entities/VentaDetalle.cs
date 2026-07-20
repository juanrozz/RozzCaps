using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class VentaDetalle
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public int GorraVariacionId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual GorraVariacione GorraVariacion { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}

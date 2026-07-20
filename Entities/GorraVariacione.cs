using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class GorraVariacione
{
    public int Id { get; set; }

    public int GorraId { get; set; }

    public int ColorId { get; set; }

    public int Stock { get; set; }

    public bool Activo { get; set; }

    public string? Talla { get; set; }

    public virtual Colore Color { get; set; } = null!;

    public virtual Gorra Gorra { get; set; } = null!;

    public virtual ICollection<GorraImagene> GorraImagenes { get; set; } = new List<GorraImagene>();

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}

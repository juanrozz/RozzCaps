using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class Colore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string CodigoHex { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<GorraVariacione> GorraVariaciones { get; set; } = new List<GorraVariacione>();
}

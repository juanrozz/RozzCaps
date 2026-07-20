using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class GorraImagene
{
    public int Id { get; set; }

    public int GorraVariacionId { get; set; }

    public string UrlImagen { get; set; } = null!;

    public bool EsPrincipal { get; set; }

    public bool Activo { get; set; }

    public virtual GorraVariacione GorraVariacion { get; set; } = null!;
}

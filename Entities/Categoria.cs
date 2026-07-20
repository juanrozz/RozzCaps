using System;
using System.Collections.Generic;

namespace RozzCaps.Entities;

public partial class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Gorra> Gorras { get; set; } = new List<Gorra>();
}

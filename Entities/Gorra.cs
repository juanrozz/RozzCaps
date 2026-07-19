using System;
using System.Collections.Generic;

namespace RozzCaps.Entidades;

public partial class Gorra
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int CategoriaId { get; set; }

    public bool Activo { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<GorraVariacione> GorraVariaciones { get; set; } = new List<GorraVariacione>();
}

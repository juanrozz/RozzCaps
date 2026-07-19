using System;
using System.Collections.Generic;

namespace RozzCaps.Entidades;

public partial class VentaEnvio
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public string ClienteNombre { get; set; } = null!;

    public string ClienteEmail { get; set; } = null!;

    public string ClienteTelefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Barrio { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string Departamento { get; set; } = null!;

    public string? CodigoPostal { get; set; }

    public virtual Venta Venta { get; set; } = null!;
}

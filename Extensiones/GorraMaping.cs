using RozzCaps.DTOs.Response;
using RozzCaps.Entidades;

namespace RozzCaps.Extensiones
{
    public static class GorraMaping
    {
        public static CrearGorraResponseDto ToResponseDto(this Gorra data)
        {
            if (data == null)
            {
                return null;
            }

            return new CrearGorraResponseDto
            {
                Nombre = data.Nombre,
                Descripcion = data.Descripcion,
                Valor = data.Precio,
                Categoria = data.CategoriaId,
                Activo = true,
                Variaciones = data.GorraVariaciones.Select(a => new GorraVariacionesResponseDto
                {
                    Color = a.ColorId,
                    Stock = a.Stock,
                    Talla = a.Talla,
                    Activo = true,
                    Imagenes = a.GorraImagenes.Select(i => new GorrasImagenesResponseDto
                    {
                        Url = i.UrlImagen,
                        ImagenPrincipal = i.EsPrincipal
                    }).ToList(),
                }).ToList()
            };
        }
    }
}

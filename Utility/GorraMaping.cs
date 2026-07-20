using Azure.Core;
using RozzCaps.DTOs.Response;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entities;
using System.Text.RegularExpressions;

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

        public static Gorra ToEntity(this CrearGorraRequestDto request) {

            if (request == null) return null;
         
            return new Gorra
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Valor,
                CategoriaId = request.Categoria,
                Activo = true,
                GorraVariaciones = request.Variaciones.Select(a =>
                {
                    string? tallaPegada = a.Talla != null
                    ? Regex.Replace(a.Talla, @"\s+", "") // 🚀 Solo borra los espacios, sin tocar mayúsculas
                    : null;

                    return new GorraVariacione
                    {
                        ColorId = a.Color,
                        Stock = a.Stock,
                        Talla = tallaPegada,
                        Activo = true,
                        GorraImagenes = a.Imagenes.Select(i => new GorraImagene
                        {
                            UrlImagen = i.Url,
                            EsPrincipal = i.ImagenPrincipal
                        }).ToList(),
                    };
                }).ToList()
            };
        }
    }
}

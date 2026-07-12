using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entidades;

namespace RozzCaps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GorrasController : ControllerBase
    {
        private readonly RozzCapsContext _dbContext;

        public GorrasController(RozzCapsContext dbContext)
        { 
            _dbContext = dbContext;
        }

        [HttpPost]

        public async Task<ActionResult> CrearGorra([FromBody] CrearGorraRequestDto request)
        {
            if (request.CategoriaId <= 0)
            {
                return BadRequest("esta actegoria no existe mano");
            }

            Categoria? existe = await _dbContext.Categorias.FirstOrDefaultAsync(e => e.Id == request.CategoriaId);

            if (existe == null) {
                return BadRequest("Esta categoria no existe paisano");
            }
            if (existe.Activo == false)
            {
                return BadRequest("Esta categoria esta inactiva");
            }

            var data = new Gorra
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                CategoriaId = request.CategoriaId,
                Activo = true,
                GorraVariaciones = request.Variaciones.Select(a => new GorraVariacione
                {
                    ColorId= a.ColorId,
                    Stock = a.Stock,
                    Sku = a.SKU,
                    Activo = true,
                    GorraImagenes = a.Imagenes.Select(i => new GorraImagene
                    {
                        UrlImagen = i.Url,
                        EsPrincipal = i.ImagenPrincipal
                    }).ToList(),
                }).ToList()
            };
            await _dbContext.AddAsync(data);
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                Mensaje = "Gorra creada con éxito en la base de datos",
                Nombre = data.Nombre,
                Precio = data.Precio
            });
        }

    }
}

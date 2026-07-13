using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RozzCaps.DTOs.Response;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entidades;
using RozzCaps.Extensiones;

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

        [HttpPost("Crear")]

        public async Task<ActionResult<CrearGorraResponseDto>> CrearGorra([FromBody] CrearGorraRequestDto request)
        {
            if (request.Categoria <= 0)
            {
                return BadRequest("esta categoria no existe mano");
            }

            Categoria? existe = await _dbContext.Categorias.FirstOrDefaultAsync(e => e.Id == request.Categoria);

            if (existe == null) {
                return BadRequest("Esta categoria no existe paisano");
            }
            if (existe.Activo == false)
            {
                return BadRequest("Esta categoria esta inactiva");
            }

            Gorra data = new Gorra
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Valor,
                CategoriaId = request.Categoria,
                Activo = true,
                GorraVariaciones = request.Variaciones.Select(a => new GorraVariacione
                {
                    ColorId= a.Color,
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

            CrearGorraResponseDto response = data.ToResponseDto();

            return Ok(new
            {
                Mensaje = "Gorra creada con éxito en la base de datos",
                Gorra = response
            });
        }

        [HttpGet("Stock")]
        public async Task<ActionResult> MostrarGorras()
        {
            List<Gorra> gorras = await _dbContext.Gorras.Where(g => g.Activo)
                .Include(g => g.GorraVariaciones.Where(g => g.Activo))
                .ThenInclude(a => a.GorraImagenes)
                .ToListAsync();

            if(gorras.Count <= 0)
            {
                return BadRequest("Error al obtener las gorras");
            }

            List<CrearGorraResponseDto> response = gorras.Select(a => a.ToResponseDto()).ToList();

            return Ok(response);
        }

    }
}

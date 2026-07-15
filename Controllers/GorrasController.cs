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

        [HttpGet]
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

        [HttpGet("buscar")]
        public async Task<ActionResult<List<CrearGorraResponseDto>>> BuscarGorras([FromQuery] string request)
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                return BadRequest(new { Mensaje = "Ingresa un término para realizar la búsqueda." });
            }

            string busqueda = request.Trim().ToLower();
            string raiz = busqueda.Length >= 4 ? busqueda.Substring(0, 3) : busqueda;
            // Intentar verificar si el usuario ingresó un número (para buscar por ID)
            bool esNumero = int.TryParse(busqueda, out int idBuscado);

            List<Gorra> resultados = await _dbContext.Gorras
                .Include(c => c.Categoria)
                .Include(g => g.GorraVariaciones.Where(v => v.Activo))
                .ThenInclude(v => v.GorraImagenes)
                .Where(g => g.Activo && (
                    (esNumero && g.Id == idBuscado) || // Si es número, evalúa el ID

                    g.Nombre.ToLower().Contains(busqueda) || // Coincidencia parcial en el Nombre
                    g.Nombre.ToLower().Contains(raiz) ||

                    (g.Categoria != null && (g.Categoria.Nombre.ToLower().Contains(busqueda) ||
                    g.Categoria.Nombre.ToLower().Contains(raiz)
                    )) ||

                    g.GorraVariaciones.Any(v => v.Sku != null && v.Sku.ToLower().Contains(busqueda)) ||

                    g.GorraVariaciones.Any(v => v.Color != null && (v.Color.Nombre.ToLower().Contains(busqueda) ||
                    v.Color.Nombre.ToLower().Contains(raiz)

                    ))
                     // Coincidencia por SKU
                ))

                .ToListAsync();

            if (resultados.Count == 0){

                return NotFound("Gorra no encontrada");
            }

            List<CrearGorraResponseDto> response = resultados.Select(g => g.ToResponseDto()).ToList();

            return Ok(response);
        }

    }
}

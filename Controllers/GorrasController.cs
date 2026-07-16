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

            Gorra data = request.ToEntity();
                
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

        [HttpGet("buscar")] //filtra los estilos de a una palabra se este escrita a medias o completa o por id
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
                    (esNumero && g.Id == idBuscado) || // por el ID

                    g.Nombre.ToLower().Contains(busqueda) || // por el Nombre
                    g.Nombre.ToLower().Contains(raiz) ||

                    (g.Categoria != null && (g.Categoria.Nombre.ToLower().Contains(busqueda) || // por el nombre de la categoria
                    g.Categoria.Nombre.ToLower().Contains(raiz)
                    )) ||

                    g.GorraVariaciones.Any(v => v.Color != null && (v.Color.Nombre.ToLower().Contains(busqueda) || // por color
                    v.Color.Nombre.ToLower().Contains(raiz) ||

                    g.GorraVariaciones.Any(t =>
                    (busqueda == "ajustable" && v.Talla == null) ||
                    (busqueda == "cerrada" && v.Talla != null) ||
                    t.Talla != null && (v.Talla.ToLower() == busqueda))

                    ))
                     
                ))

                .ToListAsync();

            if (resultados.Count == 0){

                return NotFound($"No se encontraron resultados para {request}");
            }

            List<CrearGorraResponseDto> response = resultados.Select(g => g.ToResponseDto()).ToList();

            return Ok(response);
        }

       

    }
}

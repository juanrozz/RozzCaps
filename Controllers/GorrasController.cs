using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RozzCaps.Attributes;
using RozzCaps.DTOs.Response;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entities;
using RozzCaps.Extensiones;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpGet("Catalogo")]
        public async Task<ActionResult> Catalogo()
        {
            List<Gorra> gorras = await _dbContext.Gorras.Where(g => g.Activo)
                .Include(g => g.GorraVariaciones.Where(g => g.Activo))
                .ThenInclude(a => a.GorraImagenes)
                .ToListAsync();

            if (gorras.Count <= 0)
            {
                return Ok(new List<string>());
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

            string textoLimpio = request.Trim();
            string busquedaTalla = Regex.Replace(textoLimpio, @"\s+", "").ToLower(); // busqueda talla
            // Intentar verificar si el usuario ingresó un número (para buscar por ID)
            bool esNumero = int.TryParse(textoLimpio, out int idBuscado);

            string[] palabras = textoLimpio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // se sepra por terminos las palabras

            IQueryable<Gorra> query =  _dbContext.Gorras
                .Include(c => c.Categoria)
                .Include(g => g.GorraVariaciones)
                    .ThenInclude(v => v.GorraImagenes)
                .Include(g => g.GorraVariaciones)
                    .ThenInclude(c => c.Color);

            if (esNumero)
            {
                query = query.Where(g => g.Id == idBuscado);// se prioriza la busqueda exacta por id
            }
            else
            {
            
                foreach (var palabra in palabras)
                {
                    string palabraActual = palabra.ToLower();
                    string raiz = palabraActual.Length >= 4 ? palabraActual.Substring(0, 3) : palabraActual;

                    query = query.Where(g =>
                        g.Nombre.Contains(palabraActual) ||
                        g.Nombre.Contains(raiz) ||
                        (g.Categoria != null && (g.Categoria.Nombre.Contains(palabraActual) || 
                            g.Categoria.Nombre.Contains(raiz))) ||
                        g.GorraVariaciones.Any(v =>
                            v.Activo && (
                                (v.Color != null && (v.Color.Nombre.Contains(palabraActual) || 
                                    v.Color.Nombre.Contains(raiz))) ||
                                (palabraActual == "ajustable" && v.Talla == null) ||
                                (palabraActual == "cerrada" && v.Talla != null) ||
                                (v.Talla != null && v.Talla.Contains(palabraActual)) ||
                                (v.Talla != null && v.Talla.Contains(busquedaTalla))

                            )
                        )
                    );
                }
            }

            List<Gorra> resultados = await query.Where(g => g.Activo).ToListAsync();

            if (resultados.Count == 0)
            {
                return NotFound(new { Mensaje = $"No se encontraron resultados para '{request}'" });
            }

            List<CrearGorraResponseDto> response = resultados.Select(g => g.ToResponseDto()).ToList();
            return Ok(response);
        }

       
    }
}

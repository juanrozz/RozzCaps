using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RozzCaps.Attributes;
using RozzCaps.DTOs.Request;
using RozzCaps.DTOs.Response;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entities;
using RozzCaps.Extensiones;

namespace RozzCaps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class AdminController : ControllerBase
    {
        private readonly RozzCapsContext _dbContext;
        public AdminController(RozzCapsContext dbContext) {
        
            _dbContext = dbContext;
        }

        [HttpPost("CrearGorra")]
        public async Task<ActionResult<CrearGorraResponseDto>> CrearGorra([FromBody] CrearGorraRequestDto request)
        {
            if (request.Categoria <= 0)
            {
                return BadRequest("esta categoria no existe mano");
            }

            Categoria? existe = await _dbContext.Categorias.FirstOrDefaultAsync(e => e.Id == request.Categoria);

            if (existe == null)
            {
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

        [HttpPut("ActualizarStock")]

        public async Task<IActionResult> ActualizarStock([FromBody] ActualizarStockRequestDto request)
        {
            if (request.NuevaCantidad < 0)
            {
                return BadRequest(new {Mensaje = $"El stock no puede ser una cantidad negativa paisano"});
            }

            GorraVariacione? variacion = await _dbContext.GorraVariaciones
                .FirstOrDefaultAsync(v => v.Id == request.GorraVariacionId);

            if (variacion == null)
            {
                return NotFound(new {Mensaje = $"No se encontro la variacion de gorra con el ID {request.GorraVariacionId}"});
            }

            variacion.Stock = request.NuevaCantidad;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Inventario actualizado con exito",
                GorraVariacionId = request.GorraVariacionId,
                NuevoStock = variacion.Stock
            });
        }
    }
}

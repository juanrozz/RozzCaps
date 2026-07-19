using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RozzCaps.DTOs;
using RozzCaps.DTOs.Response;
using RozzCaps.DTOs.Resquest;
using RozzCaps.Entidades;
using RozzCaps.Services;

namespace RozzCaps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly RozzCapsContext _dbContext;
        private readonly EnvioService _envioService;

        public VentasController(RozzCapsContext dbContext, EnvioService envioService)
        {
            _dbContext = dbContext;
            _envioService = envioService;
        }

        [HttpPost("CrearOrden")]
        public async Task<ActionResult<VentaResponseDto>> CrearOrden([FromBody] VentaRequestDto request)
        {
            if (request == null || !request.Items.Any())
            {
                return BadRequest(new { Mensaje = "El carrito de compras está vacío." });
            }

            using var transaccion = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                decimal subtotalGorras = 0;
                
                List<VentaDetalle> detallesParaGuardar = new();

                foreach (var item in request.Items)
                {
                    GorraVariacione? variacion = await _dbContext.GorraVariaciones
                        .Include(v => v.Gorra)
                        .FirstOrDefaultAsync(v => v.Id == item.GorraVariacionId && v.Activo && v.Gorra.Activo);

                    if (variacion == null)
                    {
                        
                        await transaccion.RollbackAsync();
                        return NotFound(new { Mensaje = $"El producto con ID {item.GorraVariacionId} no se encuentra disponible." });
                    }

                    if (variacion.Stock < item.Cantidad)
                    {
                        
                        await transaccion.RollbackAsync();
                        return BadRequest(new { Mensaje = $"El stock es insuficiente para {variacion.Gorra.Nombre}. Disponibles: {variacion.Stock}" });
                    }

                    subtotalGorras += (variacion.Gorra.Precio * item.Cantidad);
                    variacion.Stock -= item.Cantidad;

                    // Insertamos directamente el objeto del dominio
                    detallesParaGuardar.Add(new VentaDetalle
                    {
                        GorraVariacionId = variacion.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = variacion.Gorra.Precio
                    });
                }

                decimal costoEnvio = _envioService.CalcularCostoEnvio(request.DatosEnvio.Departamento, request.DatosEnvio.Ciudad, subtotalGorras);
                decimal totalFinal = subtotalGorras + costoEnvio;

                string codigoOrden = $"ROZZ-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                Venta nuevaVenta = new Venta
                {
                    CodigoOrden = codigoOrden,
                    Total = totalFinal,
                    CostoEnvio = costoEnvio,
                    Estado = "Pendiente",
                    Fecha = DateTime.UtcNow,
                    VentaEnvios = new List<VentaEnvio>
                    {
                        new VentaEnvio
                        {
                            ClienteNombre = request.DatosEnvio.ClienteNombre,
                            ClienteEmail = request.DatosEnvio.ClienteEmail,
                            ClienteTelefono = request.DatosEnvio.ClienteTelefono,
                            Direccion = request.DatosEnvio.Direccion,
                            Barrio = request.DatosEnvio.Barrio,
                            Ciudad = request.DatosEnvio.Ciudad,
                            Departamento = request.DatosEnvio.Departamento,
                            CodigoPostal = request.DatosEnvio.CodigoPostal
                        }
                    },
                  
                    VentaDetalles = detallesParaGuardar
                };

                await _dbContext.Ventas.AddAsync(nuevaVenta);
                await _dbContext.SaveChangesAsync();

               
                await transaccion.CommitAsync();

                string urlPagoSimulada = $"https://checkout.pasarela.com/pay/{codigoOrden}?total={totalFinal}";

                return Ok(new VentaResponseDto
                {
                    Mensaje = "Orden e información de envío registradas con éxito.",
                    OrderId = codigoOrden,
                    CheckoutUrl = urlPagoSimulada,
                    Total = totalFinal
                });
            }
            catch (Exception ex)
            {
                
                await transaccion.RollbackAsync();
                return StatusCode(500, new { Mensaje = $"Error interno al procesar la venta: {ex.Message}" });
            }
        }
    }
}

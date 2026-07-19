namespace RozzCaps.Services
{
    public class EnvioService
    {
        public decimal CalcularCostoEnvio(string departamento, string ciudad, decimal subtotal)
        {
            string deptpLimpio = departamento?.Trim().ToLower() ?? string.Empty;
            string ciudadLimpio = ciudad?.Trim().ToLower() ?? string.Empty;

            if (deptpLimpio == "cundinamarca" || ciudadLimpio == "bogota")
            {
                return 10000; // valor en bogota
            }

            return 18000; // ese es el valor fuera de bogota
        }
    }
}

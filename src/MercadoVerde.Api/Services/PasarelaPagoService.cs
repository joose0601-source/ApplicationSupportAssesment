using System;

namespace MercadoVerde.Api.Services
{
    // Simula un proveedor externo de cobros. En producción es un servicio HTTP de terceros.
    public class PasarelaPagoService
    {
        private static readonly Random _rng = new();

        public class ResultadoCobro
        {
            public bool Aprobado { get; set; }
            public string? Referencia { get; set; }
            public string? MotivoRechazo { get; set; }
        }

        // Lanza excepción cuando el proveedor no está disponible; devuelve Aprobado=false
        // cuando la tarjeta es rechazada.
        public ResultadoCobro Cobrar(decimal monto, string descripcion)
        {
            // ~20% de las veces simula caída del proveedor (timeout / 503).
            if (_rng.Next(0, 5) == 0)
                throw new TimeoutException("La pasarela de pago no respondió a tiempo.");

            // ~15% simula rechazo de la tarjeta.
            if (_rng.Next(0, 100) < 15)
                return new ResultadoCobro { Aprobado = false, MotivoRechazo = "Fondos insuficientes" };

            return new ResultadoCobro { Aprobado = true, Referencia = Guid.NewGuid().ToString("N") };
        }
    }
}

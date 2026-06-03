namespace MercadoVerde.Application.Abstractions;

/// <summary>Puerto hacia el proveedor externo de cobros (pasarela de pago).</summary>
public interface IPasarelaPagoService
{
    ResultadoCobro Cobrar(decimal monto, string descripcion);
}

public class ResultadoCobro
{
    public bool Aprobado { get; set; }
    public string? Referencia { get; set; }
    public string? MotivoRechazo { get; set; }
}

using FluentAssertions;
using MercadoVerde.Domain;

namespace MercadoVerde.Application.UnitTests;

/// <summary>
/// Prueba de humo del andamiaje de pruebas. Verifica invariantes neutrales del
/// dominio (no comportamiento de negocio) para no fijar los defectos del ejercicio.
/// </summary>
public class DominioTests
{
    [Fact]
    public void PedidoNuevo_ArrancaPendiente()
    {
        var pedido = new Pedido();

        pedido.Estado.Should().Be(EstadoPedido.Pendiente);
        pedido.Lineas.Should().BeEmpty();
    }
}

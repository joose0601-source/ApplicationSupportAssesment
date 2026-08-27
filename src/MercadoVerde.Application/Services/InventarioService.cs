using System.Linq;
using MercadoVerde.Application.Abstractions;

namespace MercadoVerde.Application.Services;

public class InventarioService
{
    private readonly ITiendaDbContext _db;

    public InventarioService(ITiendaDbContext db)
    {
        _db = db;
    }

    // Descuenta 'cantidad' unidades del stock del producto.
    // Este método es invocado al confirmar cada pedido.
    public void DescontarStock(int productoId, int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero.");
        }

        // se busca el producto para validar que exista.
        var producto = _db.Productos
            .FirstOrDefault(p => p.Id == productoId);

        if (producto == null)
        {
            throw new InvalidOperationException(
                $"Producto {productoId} no existe.");
        }

        // El descuento se hace de forma atómica en la BD.
        var actualizado = _db.DescontarStock(
            productoId,
            cantidad);

        if (!actualizado)
        {
            throw new InvalidOperationException(
                $"Stock insuficiente para el producto {producto.Nombre}.");
        }
    }
}

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
        var producto = _db.Productos.FirstOrDefault(p => p.Id == productoId);
        if (producto == null)
            throw new InvalidOperationException($"Producto {productoId} no existe.");

        // Se lee el stock, se valida y luego se actualiza.
        if (producto.Stock < cantidad)
            throw new InvalidOperationException(
                $"Stock insuficiente para el producto {producto.Nombre}.");

        producto.Stock = producto.Stock - cantidad;
        _db.SaveChanges();
    }
}

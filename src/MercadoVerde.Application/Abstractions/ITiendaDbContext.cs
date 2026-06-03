using Microsoft.EntityFrameworkCore;
using MercadoVerde.Domain;

namespace MercadoVerde.Application.Abstractions;

/// <summary>
/// Puerto de persistencia que expone los conjuntos de entidades de la tienda.
/// La implementación concreta (EF Core) vive en la capa de Infraestructura, de modo
/// que la capa de Aplicación no depende del proveedor de base de datos.
/// </summary>
public interface ITiendaDbContext
{
    DbSet<Producto> Productos { get; }
    DbSet<Cliente> Clientes { get; }
    DbSet<Cupon> Cupones { get; }
    DbSet<Pedido> Pedidos { get; }
    DbSet<LineaPedido> LineasPedido { get; }

    int SaveChanges();
}

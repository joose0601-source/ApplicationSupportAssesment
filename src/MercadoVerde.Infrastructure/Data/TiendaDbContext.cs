using Microsoft.EntityFrameworkCore;
using MercadoVerde.Application.Abstractions;
using MercadoVerde.Domain;

namespace MercadoVerde.Infrastructure.Data;

public class TiendaDbContext : DbContext, ITiendaDbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Cupon> Cupones => Set<Cupon>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<LineaPedido> LineasPedido => Set<LineaPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Pedido>().Property(p => p.Total).HasColumnType("decimal(18,2)");
        base.OnModelCreating(modelBuilder);
    }
    // Descuento atómico para evitar problemas de concurrencia
    public bool DescontarStock(int productoId, int cantidad)
    {
        if (Database.ProviderName?.Contains("InMemory") == true)
        {
            var producto = Productos
                .FirstOrDefault(p => p.Id == productoId);

            if (producto == null || producto.Stock < cantidad)
                return false;

            producto.Stock -= cantidad;
            SaveChanges();

            return true;
        }

        var filasAfectadas = Productos
            .Where(p =>
                p.Id == productoId &&
                p.Stock >= cantidad)
            .ExecuteUpdate(setters =>
                setters.SetProperty(
                    p => p.Stock,
                    p => p.Stock - cantidad));

        return filasAfectadas == 1;
    }
}

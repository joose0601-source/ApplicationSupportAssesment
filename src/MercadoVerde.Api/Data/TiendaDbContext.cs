using System;
using Microsoft.EntityFrameworkCore;
using MercadoVerde.Api.Models;

namespace MercadoVerde.Api.Data
{
    public class TiendaDbContext : DbContext
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
    }

    public static class SeedData
    {
        public static void Inicializar(TiendaDbContext db)
        {
            db.Database.EnsureCreated();
            if (db.Productos.Any()) return;

            db.Productos.AddRange(
                new Producto { Nombre = "Audífonos Bluetooth", Precio = 25.00m, Stock = 5 },
                new Producto { Nombre = "Teclado Mecánico", Precio = 49.90m, Stock = 3 },
                new Producto { Nombre = "Mouse Inalámbrico", Precio = 15.50m, Stock = 10 },
                new Producto { Nombre = "Monitor 24\"", Precio = 180.00m, Stock = 2 }
            );

            db.Clientes.AddRange(
                new Cliente { Nombre = "Ana López", Email = "ana@example.com" },
                new Cliente { Nombre = "Bruno Díaz", Email = null } // cliente sin email (caso límite)
            );

            db.Cupones.AddRange(
                new Cupon { Codigo = "BIENVENIDA10", PorcentajeDescuento = 10m,
                            FechaExpiracionUtc = DateTime.UtcNow.AddDays(30), Activo = true },
                new Cupon { Codigo = "EXPIRADO", PorcentajeDescuento = 20m,
                            FechaExpiracionUtc = DateTime.UtcNow.AddDays(-1), Activo = true }
            );

            db.SaveChanges();
        }
    }
}

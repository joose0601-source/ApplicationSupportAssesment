using System.Linq;
using MercadoVerde.Application.Abstractions;

namespace MercadoVerde.Application.Services;

public class ReporteService
{
    private readonly ITiendaDbContext _db;

    public ReporteService(ITiendaDbContext db)
    {
        _db = db;
    }

    public class FilaReporte
    {
        public int PedidoId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int CantidadArticulos { get; set; }
        public decimal Total { get; set; }
    }

    // Genera el reporte de ventas de un rango de fechas.
    // En producción la tabla Pedidos tiene cientos de miles de filas.

    // Se genera el reporte en una sola consulta para evitar N+1.
    public List<FilaReporte> GenerarReporteVentas(
        DateTime desdeUtc,
        DateTime hastaUtc)
    {
        var reporte = from pedido in _db.Pedidos
                      where pedido.FechaUtc >= desdeUtc &&
                            pedido.FechaUtc <= hastaUtc
                      join cliente in _db.Clientes
                          on pedido.ClienteId equals cliente.Id
                          into clientes
                      from cliente in clientes.DefaultIfEmpty()
                      join linea in _db.LineasPedido
                          on pedido.Id equals linea.PedidoId
                          into lineas
                      select new FilaReporte
                      {
                          PedidoId = pedido.Id,
                          Cliente = cliente != null
                              ? cliente.Nombre
                              : "(desconocido)",
                          CantidadArticulos = lineas
                              .Sum(l => (int?)l.Cantidad) ?? 0,
                          Total = pedido.Total
                      };

        return reporte.ToList();
    }
}

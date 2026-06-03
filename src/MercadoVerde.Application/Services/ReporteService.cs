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
    public List<FilaReporte> GenerarReporteVentas(DateTime desdeUtc, DateTime hastaUtc)
    {
        var pedidos = _db.Pedidos
            .Where(p => p.FechaUtc >= desdeUtc && p.FechaUtc <= hastaUtc)
            .ToList();

        var filas = new List<FilaReporte>();
        foreach (var pedido in pedidos)
        {
            // Por cada pedido se vuelve a la base de datos a traer sus líneas
            // y el nombre del cliente.
            var lineas = _db.LineasPedido.Where(l => l.PedidoId == pedido.Id).ToList();
            var cliente = _db.Clientes.FirstOrDefault(c => c.Id == pedido.ClienteId);

            filas.Add(new FilaReporte
            {
                PedidoId = pedido.Id,
                Cliente = cliente?.Nombre ?? "(desconocido)",
                CantidadArticulos = lineas.Sum(l => l.Cantidad),
                Total = pedido.Total
            });
        }

        return filas;
    }
}

using Microsoft.AspNetCore.Mvc;
using MercadoVerde.Application.Abstractions;
using MercadoVerde.Application.Dtos;
using MercadoVerde.Application.Services;

namespace MercadoVerde.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;
    public ProductosController(IProductoRepository repo) => _repo = repo;

    // GET /api/productos/buscar?termino=mouse
    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] string termino)
    {
        var resultado = _repo.BuscarPorNombre(termino ?? string.Empty);
        return Ok(resultado);
    }
}

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidos;
    public PedidosController(PedidoService pedidos) => _pedidos = pedidos;

    // POST /api/pedidos
    [HttpPost]
    public IActionResult Crear([FromBody] CrearPedidoDto dto)
    {
        var pedido = _pedidos.CrearPedido(dto);
        return Ok(pedido);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ReporteService _reportes;
    public ReportesController(ReporteService reportes) => _reportes = reportes;

    // GET /api/reportes/ventas?desde=2025-01-01&hasta=2025-12-31
    [HttpGet("ventas")]
    public IActionResult Ventas([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        var desdeUtc = desde.Date;
        var hastaUtc = hasta.Date.AddDays(1);

        return Ok(_reportes.GenerarReporteVentas(desdeUtc, hastaUtc));
    }
}

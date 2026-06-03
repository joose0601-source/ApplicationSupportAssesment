using System;
using Microsoft.AspNetCore.Mvc;
using MercadoVerde.Api.Data;
using MercadoVerde.Api.Dtos;
using MercadoVerde.Api.Services;

namespace MercadoVerde.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoRepository _repo;
        public ProductosController(ProductoRepository repo) => _repo = repo;

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
            return Ok(_reportes.GenerarReporteVentas(desde, hasta));
        }
    }
}

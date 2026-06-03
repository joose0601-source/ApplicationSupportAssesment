using System.Net;
using FluentAssertions;

namespace MercadoVerde.WebApi.IntegrationTests;

/// <summary>
/// Prueba de humo de integración: la aplicación arranca, siembra la base y el
/// endpoint de catálogo responde 200 ante un término normal.
/// </summary>
public class CatalogoEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public CatalogoEndpointTests(TestWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task BuscarProductos_DevuelveOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/productos/buscar?termino=mouse");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

using System.Net;
using System.Net.Http.Json;

namespace Orders.API.Clients;

public class ProductsApiClient : IProductsApiClient
{
    private readonly HttpClient _http;

    public ProductsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var response = await _http.GetAsync($"/api/products/{productId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}
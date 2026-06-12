using System.Net.Http.Json;

namespace Products.API.Clients;

public class OrdersApiClient : IOrdersApiClient
{
    private readonly HttpClient _http;

    public OrdersApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<int> GetActiveOrdersCountAsync(Guid productId)
    {
        var response = await _http.GetAsync($"/api/orders/active-count?productoId={productId}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ActiveOrdersCountDto>();
        return result?.OrdenesActivas ?? 0;
    }
}
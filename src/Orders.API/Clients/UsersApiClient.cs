using System.Net;

namespace Orders.API.Clients;

public class UsersApiClient : IUsersApiClient
{
    private readonly HttpClient _http;

    public UsersApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var response = await _http.GetAsync($"/api/users/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;


        response.EnsureSuccessStatusCode();

        return true;
    }
}
namespace Products.API.Clients;

public interface IOrdersApiClient
{
	Task<int> GetActiveOrdersCountAsync(Guid productId);
}
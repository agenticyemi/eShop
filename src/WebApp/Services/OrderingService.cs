using System.Net;

namespace eShop.WebApp.Services;

public class OrderingService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "/api/Orders/";

    public Task<OrderRecord[]> GetOrders()
    {
        return httpClient.GetFromJsonAsync<OrderRecord[]>(remoteServiceBaseUrl)!;
    }

    public Task CreateOrder(CreateOrderRequest request, Guid requestId)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
        requestMessage.Headers.Add("x-requestid", requestId.ToString());
        requestMessage.Content = JsonContent.Create(request);
        return httpClient.SendAsync(requestMessage);
    }

    public Task<OrderReturnsRecord?> GetOrderReturns(int orderNumber)
    {
        return httpClient.GetFromJsonAsync<OrderReturnsRecord>($"{remoteServiceBaseUrl}{orderNumber}/returns");
    }

    public async Task<RequestReturnResult> RequestReturn(int orderNumber, RequestReturnRequest request, Guid requestId)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{remoteServiceBaseUrl}{orderNumber}/returns");
        requestMessage.Headers.Add("x-requestid", requestId.ToString());
        requestMessage.Content = JsonContent.Create(request);
        var response = await httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            return new RequestReturnResult(true, null);
        }

        // The API answers business rule violations with a 400 and a plain message meant for the shopper
        var errorMessage = response.StatusCode == HttpStatusCode.BadRequest
            ? await response.Content.ReadFromJsonAsync<string>()
            : null;

        return new RequestReturnResult(false, errorMessage ?? "We couldn't process your return request. Please try again.");
    }
}

public record OrderRecord(
    int OrderNumber,
    DateTime Date,
    string Status,
    decimal Total,
    string? ReturnStatus);

public record OrderReturnsRecord(
    int OrderNumber,
    string OrderStatus,
    bool CanRequestReturn,
    ReturnableItemRecord[] Items,
    ReturnRecord[] Returns);

public record ReturnableItemRecord(
    int ProductId,
    string ProductName,
    string? PictureUrl,
    int Units,
    int ReturnableUnits);

public record ReturnRecord(
    int ReturnNumber,
    int ProductId,
    string ProductName,
    int Units,
    string Reason,
    string Status,
    DateTime RequestDate);

public record RequestReturnRequest(
    int ProductId,
    int Units,
    string Reason);

public record RequestReturnResult(
    bool Succeeded,
    string? ErrorMessage);

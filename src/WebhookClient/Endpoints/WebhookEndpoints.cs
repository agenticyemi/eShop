using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace eShop.WebhookClient.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        const string webhookCheckHeader = "X-eshop-whtoken";

        var configuration = app.ServiceProvider.GetRequiredService<IConfiguration>();
        bool.TryParse(configuration["ValidateToken"], out var validateToken);
        var tokenToValidate = configuration["WebhookClientOptions:Token"];

        app.MapMethods("/check", [HttpMethods.Options], Results<Ok, BadRequest<string>> ([FromHeader(Name = webhookCheckHeader)] string value, HttpResponse response) =>
        {
            if (!validateToken || value == tokenToValidate)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    response.Headers.Append(webhookCheckHeader, value);
                }

                return TypedResults.Ok();
            }

            return TypedResults.BadRequest("Invalid token");
        });

        app.MapPost("/webhook-received", async (WebhookData hook, HttpRequest request, ILogger<Program> logger, HooksRepository hooksRepository) =>
        {
            var token = request.Headers[webhookCheckHeader];

            // LoggerExtensions formats a header StringValues by joining its values with ", ".
            WebhookEndpointsLog.LogReceivedHook(logger, string.Join(", ", (IEnumerable<string>)token), tokenToValidate, validateToken);

            if (!validateToken || tokenToValidate == token)
            {
                WebhookEndpointsLog.LogReceivedHookProcessing(logger);
                var newHook = new WebHookReceived()
                {
                    Data = hook.Payload,
                    When = hook.When,
                    Token = token
                };
                await hooksRepository.AddNew(newHook);
                WebhookEndpointsLog.LogReceivedHookProcessed(logger);
                return Results.Ok(newHook);
            }

            WebhookEndpointsLog.LogReceivedHookNotProcessed(logger);
            return Results.BadRequest();
        });

        return app;
    }
}

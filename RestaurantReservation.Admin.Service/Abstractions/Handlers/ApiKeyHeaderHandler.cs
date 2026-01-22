using RestaurantReservation.Common;
using SamSoft.Common.ExtensionMethods;

namespace RestaurantReservation.Admin.Service.Abstractions.Handlers;

public class ApiKeyHeaderHandler() : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiKey = GuidGenerator.GenerateTimeBasedGuid(DateTime.UtcNow);
        request.Headers.Add(HeaderConstants.ApiKeyHeaderName, apiKey.ToString());
        return await base.SendAsync(request, cancellationToken);
    }
}

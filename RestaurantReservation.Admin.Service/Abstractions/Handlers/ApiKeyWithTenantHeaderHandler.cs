using RestaurantReservation.Common;
using SamSoft.Common.ExtensionMethods;

namespace RestaurantReservation.Admin.Service.Abstractions.Handlers;

public class ApiKeyWithTenantHeaderHandler(IUserStateService userStateService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiKey = GuidGenerator.GenerateTimeBasedGuid(DateTime.UtcNow);
        // TenantId - GetTenantId will check cache first, then browser storage
        // The cache should be populated by the component before API calls are made
        var tenantId = await userStateService.GetTenantId(cancellationToken);
        var key = $"{apiKey}@{tenantId}";
        request.Headers.Add(HeaderConstants.ApiKeyHeaderName, key);
        return await base.SendAsync(request, cancellationToken);
    }
}

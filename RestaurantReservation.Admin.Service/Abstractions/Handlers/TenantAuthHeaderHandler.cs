using RestaurantReservation.Common;
using System.Net.Http.Headers;

namespace RestaurantReservation.Admin.Service.Abstractions.Handlers;

public class TenantAuthHeaderHandler(IUserStateService userStateService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await userStateService.GetAccessToken(cancellationToken);
        var tenant = await userStateService.GetTenantId(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add(HeaderConstants.TenantIdHeaderName, tenant.ToString());
        return await base.SendAsync(request, cancellationToken);
    }
}

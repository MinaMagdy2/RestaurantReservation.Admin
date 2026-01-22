using Microsoft.Extensions.Configuration;

namespace RestaurantReservation.Admin.Service.SetupData;

public class ApiServerSetup : IConfigureOptions<ApiServerOptions>
{
    private const string SectionName = "ServerSettings";
    private readonly IConfiguration _configuration;
    public ApiServerSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApiServerOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

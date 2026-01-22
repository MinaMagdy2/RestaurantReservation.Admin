using RestaurantReservation.Admin.Service.ViewModels.Items;

namespace RestaurantReservation.Admin.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<ApiServerSetup>();
        services.AddServices();
        services.AddViewModels();
        // Register HTTP message handlers as Scoped to match HTTP client lifetime
        // This ensures they share the same scope as the HTTP clients and can access scoped services
        services.AddScoped<ApiKeyHeaderHandler>()
            .AddScoped<ApiKeyWithTenantHeaderHandler>()
            .AddScoped<TenantAuthHeaderHandler>();
        services.AddApiServices(configuration);
        return services;
    }
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string address)
    {
        services.ConfigureOptions<ApiServerSetup>();
        services.AddServices();
        services.AddViewModels();
        // Register HTTP message handlers as Scoped to match HTTP client lifetime
        // This ensures they share the same scope as the HTTP clients and can access scoped services
        services.AddScoped<ApiKeyHeaderHandler>()
            .AddScoped<ApiKeyWithTenantHeaderHandler>()
            .AddScoped<TenantAuthHeaderHandler>();
        services.AddApiServices(address);
        return services;
    }
    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ILoginService, LoginService>()
            .AddScoped<ILanguageService, LanguageService>()
            .AddScoped<IRestaurantService, RestaurantService>()
            .AddScoped<IItemService, ItemService>()
            .AddScoped<IComsysService, ComsysService>();
    }
    private static void AddViewModels(this IServiceCollection services)
    {
        services.AddScoped<ILoginReactiveViewModel, LoginReactiveViewModel>()
            .AddScoped<ILanguageListViewModel, LanguageListViewModel>()
            .AddScoped<IEditLanguageViewModel, EditLanguageViewModel>()
            .AddScoped<IAddLanguageNationalitiesViewModel, AddLanguageNationalitiesViewModel>()
            .AddScoped<IRestaurantListViewModel, RestaurantListViewModel>()
            .AddScoped<IEditRestaurantViewModel, EditRestaurantViewModel>()
             .AddScoped<IItemListViewModel, ItemListViewModel>()
            .AddScoped<IAddItemViewModel, AddEditItemViewModel>()
          .AddScoped<IDeletItemViewModel, DeletItemViewModel>()
           .AddScoped<IDeletRestaurantViewModel, DeletRestaurantViewModel>();
    }
}
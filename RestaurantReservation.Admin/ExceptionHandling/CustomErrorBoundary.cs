using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace RestaurantReservation.Admin.ExceptionHandling;

public class CustomErrorBoundary : ErrorBoundary
{
    [Inject]
    private IHostEnvironment Env { get; set; } = null!;
    protected override Task OnErrorAsync(Exception exception)
    {
        Log.Error(messageTemplate: "Custom Error Boundary: {Source} - {Exception}", exception.Source, exception.Message);
        return Env.IsDevelopment() ?
            base.OnErrorAsync(exception) :
            Task.CompletedTask;
    }
}

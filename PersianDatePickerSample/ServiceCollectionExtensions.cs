using Microsoft.Extensions.DependencyInjection;
using TMDatePicker.Infrastructure;

namespace TMDatePicker;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTMDatePickerService(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        services.AddScoped<PersianDatePickerJsInterop>();
        return services;
    }
}
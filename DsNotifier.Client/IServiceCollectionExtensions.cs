using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DsNotifier.Client;

public static class IServiceCollectionExtensions
{
    public static void AddDsNotifier(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            services.AddSingleton<IDsNotifierClient, DsNotifierMassTransitClient>();
            
            var options = configuration.GetSection(DsNotifierOptions.SECTION).Get<DsNotifierOptions>() ?? throw new("No DsNotifier options");
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.Url, h => 
                {
                    h.Username(options.Username);
                    h.Password(options.Key);
                });
            });
        });
    }
}

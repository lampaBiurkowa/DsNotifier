using DsLauncher.Events;
using MassTransit;

namespace DsNotifier.Client;

public interface IDsNotifierClient
{
    Task SendDsLauncherPurchasedEvent(PurchasedEvent e, CancellationToken ct);
    Task SendGeneric<T>(T e, CancellationToken ct) where T : class;
}

class DsNotifierMassTransitClient(IPublishEndpoint publishEndpoint) : IDsNotifierClient
{
    public async Task SendDsLauncherPurchasedEvent(PurchasedEvent e, CancellationToken ct) => await publishEndpoint.Publish(e, ct);
    public async Task SendGeneric<T>(T e, CancellationToken ct) where T : class => await publishEndpoint.Publish(e, typeof(T), ct);
}

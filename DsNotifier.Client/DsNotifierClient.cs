using DsLauncher.Events;
using MassTransit;

namespace DsNotifier.Client;

public interface IDsNotifierClient
{
    Task SendDsLauncherPurchasedEvent(PurchasedEvent e, CancellationToken ct);
}

class DsNotifierMassTransitClient(IPublishEndpoint publishEndpoint) : IDsNotifierClient
{
    public async Task SendDsLauncherPurchasedEvent(PurchasedEvent e, CancellationToken ct) => await publishEndpoint.Publish(e, ct);
}

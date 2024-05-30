using MassTransit;

namespace DsNotifier.Client;

public interface IDsNotifierClient
{
    Task SendGeneric(object e, Type type, CancellationToken ct);
}

class DsNotifierMassTransitClient(IPublishEndpoint publishEndpoint) : IDsNotifierClient
{
    public async Task SendGeneric(object e, Type type, CancellationToken ct) => await publishEndpoint.Publish(e, type, ct);
}

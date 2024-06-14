
using DsNotifier.Server.Options;
using FluentEmail.Core;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DsNotifier.Server.Consumers;

class DsNotifierSendEmailConsumer(IFluentEmail fluentEmail, IOptions<EmailOptions> options) : IConsumer<SendEmailEvent>
{
    readonly EmailOptions options = options.Value;

    public async Task Consume(ConsumeContext<SendEmailEvent> ctx)
    {
        var msg = ctx.Message;
        if (options.SendToEmailOverride != null)
            msg.RecipentEmail = options.SendToEmailOverride;

        await fluentEmail.To(msg.RecipentEmail)
            .Subject(msg.Subject)
            .Body(msg.BodyHtml, true)
            .SendAsync(ctx.CancellationToken);
    }
}
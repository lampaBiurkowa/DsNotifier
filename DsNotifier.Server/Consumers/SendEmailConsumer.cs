
using FluentEmail.Core;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class SendEmailConsumer(IFluentEmail fluentEmail) : IConsumer<SendEmailEvent>
{
    public async Task Consume(ConsumeContext<SendEmailEvent> ctx)
    {
        var msg = ctx.Message;
        await fluentEmail.To("mrocznaklawaitura@gmail.com")//msg.RecipentEmail)
            .Subject("short announcement")
            .Body(msg.BodyHtml)
            .SendAsync();
    }
}
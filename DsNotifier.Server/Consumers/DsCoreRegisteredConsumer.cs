using DotLiquid;
using DsCore.ApiClient;
using DsCore.Events;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class DsCoreRegisteredConsumer(DsCoreClientFactory dsCore) : IConsumer<RegisteredEvent>
{
    public async Task Consume(ConsumeContext<RegisteredEvent> ctx)
    {
        var msg = ctx.Message;
        var user = await dsCore.CreateClient(string.Empty).User_Get2Async(msg.UserGuid, ctx.CancellationToken);

        var emailTemplate = Template.Parse(EmailLoader.LoadEmailTemplate<DsCoreRegisteredConsumer>());
        var bodyHtml = emailTemplate.Render(Hash.FromAnonymousObject(new
        {
            UserName = user.Name,
        }));

        await ctx.Publish(new SendEmailEvent
        {
            Subject = "Registered",
            BodyHtml = bodyHtml,
            RecipentEmail = user.Email
        }, ctx.CancellationToken);
    }
}
using DotLiquid;
using DsCore.ApiClient;
using DsLauncher.ApiClient;
using DsLauncher.Events;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class DsLauncherPurchasedConsumer(DsCoreClientFactory dsCore, DsLauncherClientFactory dsLauncher) : IConsumer<PurchasedEvent>
{
    public async Task Consume(ConsumeContext<PurchasedEvent> ctx)
    {
        var msg = ctx.Message;
        var userTask = dsCore.CreateClient(string.Empty).User_Get2Async(msg.UserGuid, ctx.CancellationToken);
        var productTask = dsLauncher.CreateClient(string.Empty).Product_Get2Async(msg.ProductGuid, ctx.CancellationToken);
        await Task.WhenAll([userTask, productTask]);

        var emailTemplate = Template.Parse(EmailLoader.LoadEmailTemplate<DsLauncherPurchasedConsumer>());
        var bodyHtml = emailTemplate.Render(Hash.FromAnonymousObject(new
        {
            UserName = userTask.Result.Name,
            ProductName = productTask.Result.Name
        }));

        await ctx.Publish(new SendEmailEvent
        {
            BodyHtml = bodyHtml,
            RecipentEmail = userTask.Result.Email
        }, ctx.CancellationToken);
    }
}
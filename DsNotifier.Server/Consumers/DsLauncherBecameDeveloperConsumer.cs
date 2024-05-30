using DotLiquid;
using DsCore.ApiClient;
using DsLauncher.ApiClient;
using DsLauncher.Events;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class DsLauncherBecameDeveloperConsumer(DsCoreClientFactory dsCore, DsLauncherClientFactory dsLauncher) : IConsumer<BecameDeveloperEvent>
{
    public async Task Consume(ConsumeContext<BecameDeveloperEvent> ctx)
    {
        var msg = ctx.Message;
        var userTask = dsCore.CreateClient(string.Empty).User_Get2Async(msg.UserGuid, ctx.CancellationToken);
        var developerTask = dsLauncher.CreateClient(string.Empty).Developer_Get2Async(msg.DeveloperGuid, ctx.CancellationToken);
        await Task.WhenAll([userTask, developerTask]);

        var emailTemplate = Template.Parse(EmailLoader.LoadEmailTemplate<DsLauncherBecameDeveloperConsumer>());
        var bodyHtml = emailTemplate.Render(Hash.FromAnonymousObject(new
        {
            UserName = userTask.Result.Name,
            DeveloperName = developerTask.Result.Name
        }));

        await ctx.Publish(new SendEmailEvent
        {
            Subject = "Developer access gained",
            BodyHtml = bodyHtml,
            RecipentEmail = userTask.Result.Email
        }, ctx.CancellationToken);
    }
}
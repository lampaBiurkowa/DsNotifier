using DotLiquid;
using DsCore.ApiClient;
using DsCore.Events;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class DsCoreToppedUpConsumer(DsCoreClientFactory dsCore) : IConsumer<ToppedUpEvent>
{
    public async Task Consume(ConsumeContext<ToppedUpEvent> ctx)
    {
        var msg = ctx.Message;
        var userTask = dsCore.CreateClient(string.Empty).User_Get2Async(msg.UserGuid, ctx.CancellationToken);
        var currencyTask = dsCore.CreateClient(string.Empty).Currency_GetCurrencyAsync(msg.CurrecyGuid, ctx.CancellationToken);
        await Task.WhenAll([userTask, currencyTask]);

        var emailTemplate = Template.Parse(EmailLoader.LoadEmailTemplate<DsCoreToppedUpConsumer>());
        var bodyHtml = emailTemplate.Render(Hash.FromAnonymousObject(new
        {
            UserName = userTask.Result.Name,
            Currency = currencyTask.Result.Symbol,
            msg.Value,
        }));

        await ctx.Publish(new SendEmailEvent
        {
            Subject = "Verification code",
            BodyHtml = bodyHtml,
            RecipentEmail = userTask.Result.Email
        }, ctx.CancellationToken);
    }
}
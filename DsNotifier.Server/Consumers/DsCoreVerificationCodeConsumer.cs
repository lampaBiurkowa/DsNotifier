using DotLiquid;
using DsCore.ApiClient;
using DsCore.Events;
using MassTransit;

namespace DsNotifier.Server.Consumers;

class DsCoreVerificationCodeConsumer(DsCoreClientFactory dsCore) : IConsumer<VerificationCodeEvent>
{
    public async Task Consume(ConsumeContext<VerificationCodeEvent> ctx)
    {
        var msg = ctx.Message;
        var user = await dsCore.CreateClient(string.Empty).User_Get2Async(msg.UserGuid, ctx.CancellationToken);

        var emailTemplate = Template.Parse(EmailLoader.LoadEmailTemplate<DsCoreVerificationCodeConsumer>());
        var bodyHtml = emailTemplate.Render(Hash.FromAnonymousObject(new
        {
            UserName = user.Name,
            msg.VerificationCode,
        }));

        await ctx.Publish(new SendEmailEvent
        {
            Subject = "Verification code",
            BodyHtml = bodyHtml,
            RecipentEmail = user.Email
        }, ctx.CancellationToken);
    }
}
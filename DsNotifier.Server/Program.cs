using System.Net;
using System.Net.Mail;
using System.Reflection;
using DsCore.ApiClient;
using DsLauncher.ApiClient;
using DsNotifier.Server.Consumers;
using DsNotifier.Server.Options;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddDsCore(builder.Services);
builder.Configuration.AddDsLauncher(builder.Services);
var emailOptions = builder.Configuration.GetSection(EmailOptions.SECTION).Get<EmailOptions>() ?? throw new("No email options");

builder.Services.AddFluentEmail(emailOptions.FromEmail).AddSmtpSender(new SmtpClient
{
    Host = "smtp-mail.outlook.com",
    Port = 587,
    EnableSsl = true,
    UseDefaultCredentials = false,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Credentials = new NetworkCredential(emailOptions.Username, emailOptions.Password)
});

builder.Services.AddOptions<EmailOptions>().Bind(builder.Configuration.GetSection(EmailOptions.SECTION));

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumers(Assembly.GetExecutingAssembly()); //todo doesnt work
    x.AddConsumer<DsLauncherPurchasedConsumer>();
    x.AddConsumer<DsLauncherSubscribedConsumer>();
    x.AddConsumer<DsLauncherUnsubscribedConsumer>();
    x.AddConsumer<DsLauncherBecameDeveloperConsumer>();
    x.AddConsumer<DsCoreRegisteredConsumer>();
    x.AddConsumer<DsCoreVerificationCodeConsumer>();
    x.AddConsumer<DsCoreToppedUpConsumer>();
    x.AddConsumer<DsNotifierSendEmailConsumer>();
    var options = builder.Configuration.GetSection(MassTransitOptions.SECTION).Get<MassTransitOptions>() ?? throw new("No mass transit options");

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(options.Url, h => 
        {
            h.Username(options.Username);
            h.Password(options.Key);
        });

        cfg.ReceiveEndpoint(nameof(DsLauncher), e =>
        {
            e.ConfigureConsumer<DsLauncherPurchasedConsumer>(context);
            e.ConfigureConsumer<DsLauncherSubscribedConsumer>(context);
            e.ConfigureConsumer<DsLauncherUnsubscribedConsumer>(context);
            e.ConfigureConsumer<DsLauncherBecameDeveloperConsumer>(context);
        });

        cfg.ReceiveEndpoint(nameof(DsCore), e =>
        {
            e.ConfigureConsumer<DsCoreRegisteredConsumer>(context);
            e.ConfigureConsumer<DsCoreVerificationCodeConsumer>(context);
            e.ConfigureConsumer<DsCoreToppedUpConsumer>(context);
        });

        cfg.ReceiveEndpoint(nameof(DsNotifier), e =>
        {
            e.ConfigureConsumer<DsNotifierSendEmailConsumer>(context);
        });
    });
    
});

var app = builder.Build();
app.Run();
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
    x.AddConsumer<SendEmailConsumer>();
    var options = builder.Configuration.GetSection(MassTransitOptions.SECTION).Get<MassTransitOptions>() ?? throw new("No mass transit options");

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(options.Url, h => 
        {
            h.Username(options.Username);
            h.Password(options.Key);
        });
        // cfg.ConfigureEndpoints(context);
        cfg.ReceiveEndpoint(nameof(DsLauncher), e =>
        {
            e.ConfigureConsumer<DsLauncherPurchasedConsumer>(context);
        });

        cfg.ReceiveEndpoint(nameof(DsNotifier), e =>
        {
            e.ConfigureConsumer<SendEmailConsumer>(context);
        });
    });
    
});

var app = builder.Build();
app.Run();
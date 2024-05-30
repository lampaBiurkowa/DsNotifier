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
    Host = "smtp.gmail.com",
    Port = 587,
    EnableSsl = true,
    UseDefaultCredentials = false,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Credentials = new NetworkCredential(emailOptions.Username, emailOptions.Password)
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetExecutingAssembly());
    var options = builder.Configuration.GetSection(MassTransitOptions.SECTION).Get<MassTransitOptions>() ?? throw new("No mass transit options");

    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host(options.Url, h => 
        {
            h.Username(options.Username);
            h.Password(options.Key);
        });
        cfg.ConfigureEndpoints(context);
        cfg.ReceiveEndpoint(nameof(DsLauncher), e =>
        {
            e.Consumer<DsLauncherPurchasedConsumer>(context);
        });

        cfg.ReceiveEndpoint(nameof(DsNotifier), e =>
        {
            e.Consumer<SendEmailConsumer>(context);
        });
    });
    
});

var app = builder.Build();
app.Run();
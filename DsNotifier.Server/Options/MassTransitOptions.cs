namespace DsNotifier.Server.Options;

public class MassTransitOptions
{
    public const string SECTION = "MassTransit";
    public required string Key { get; set; }
    public required string Url { get; set; }
    public required string Username { get; set; }
}
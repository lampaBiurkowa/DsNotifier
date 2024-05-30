namespace DsNotifier.Server.Options;

public class EmailOptions
{
    public const string SECTION = "Email";
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromEmail { get; set; }
}
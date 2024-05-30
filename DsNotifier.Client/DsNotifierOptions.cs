namespace DsNotifier.Client;

public class DsNotifierOptions
{
    public const string SECTION = nameof(DsNotifier);
    
    public required string Key { get; set; }
    public required string Url { get; set; }
    public required string Username { get; set; }
}
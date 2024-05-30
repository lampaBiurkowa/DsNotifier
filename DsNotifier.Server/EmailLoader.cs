namespace DsNotifier.Server;

static class EmailLoader
{
    public static string LoadEmailTemplate<T>()
    {
        var typeName = typeof(T).Name;
        var trimmedTypeName = typeName.EndsWith("Consumer") ? typeName[..^"Consumer".Length] : typeName;

        return File.ReadAllText($"Emails/{trimmedTypeName}.html");
    }
}
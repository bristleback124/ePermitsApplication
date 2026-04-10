namespace ePermitsApp.Helpers;

public class FileStorageSettings
{
    public string Provider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
}

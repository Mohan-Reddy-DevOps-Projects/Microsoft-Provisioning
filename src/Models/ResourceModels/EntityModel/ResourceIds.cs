namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Class to compute resourceId from resource
/// </summary>
public static class ResourceIds
{
    /// <summary>
    /// The processing storage
    /// </summary>
    public const string ProcessingStorage = "processingStorage/{0}";

    /// <summary>
    /// The catalog config
    /// </summary>
    public const string CatalogConfig = "catalogConfig/{0}";

    /// <summary>
    /// Create a formatted resource id.
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Create(string resourceType, string[] args)
    {
        return string.Format(resourceType, args);
    }
}

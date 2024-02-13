using System.ServiceModel.Syndication;

namespace FeedBot;

public static class SyndicationItemExtensions
{
    public static string? ReadElementExtension(this SyndicationItem item, string extensionName, string extensionNamespace)
    {
        var elementExtensions = item.ElementExtensions
            .ReadElementExtensions<string>(extensionName, extensionNamespace);
        return elementExtensions.FirstOrDefault();
    }
}
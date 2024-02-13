using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using FeedBot.Models;

namespace FeedBot;

public class FeedProcessor(ILogger<DiscordBot> logger) : IFeedProcessor
{
    public RssFeed ProcessFeed(string feedUrl)
    {
        var reader = XmlReader.Create(feedUrl);
        var feed = SyndicationFeed.Load(reader);
        return new RssFeed
        {
            Id = feed.Id,
            Title = feed.Title.Text,
            IdentifyingLink = feed.Links.FirstOrDefault()?.Uri.AbsoluteUri,
            Links = feed.Links,
            ImageUrl = feed.ImageUrl.AbsoluteUri,
            Items = feed.Items.Select(i =>
            {
                var link = i.Links.First().Uri.AbsoluteUri;

                return new RssItem
                {
                    Title = i.Title.Text,
                    Summary = i.Summary.Text,
                    Url = link,
                };
            }).ToList()
        };
    }


    public bool IsValidFeed(string feedUrl)
    {
        try
        {
            var feed = ProcessFeed(feedUrl);
            return !string.IsNullOrEmpty(feed.Title);
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not process feed with URL {feedUrl}: {ex.Message}");
        }

        return false;
    }
}

public class RssFeed
{
    public string Id { get; set; }
    public string Title { get; set; }
    public IReadOnlyList<RssItem> Items { get; set; }
    public string? IdentifyingLink { get; set; }
    public Collection<SyndicationLink> Links { get; set; }
    public string ImageUrl { get; set; }
}
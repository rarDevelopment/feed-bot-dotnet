using FeedBot.Models;

namespace FeedBot;

public interface IFeedProcessor
{
    RssFeed ProcessFeed(string feedUrl);
    bool IsValidFeed(string feedUrl);
}
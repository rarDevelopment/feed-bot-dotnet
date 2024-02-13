using FeedBot.Models;

namespace FeedBot.DataLayer;

public interface IFeedDataLayer
{
    Task<Feed?> GetFeed(string guid);
    Task<IReadOnlyList<Feed>> GetAllActiveFeeds();
    Task<bool> InsertFeed(Feed feed);
}
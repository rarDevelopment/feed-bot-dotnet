using System.Globalization;
using FeedBot.DataLayer.SchemaModels;
using FeedBot.Models;
using MongoDB.Driver;

namespace FeedBot.DataLayer;

public class FeedDataLayer : IFeedDataLayer
{
    private readonly IMongoCollection<FeedEntity> _feedCollection;

    public FeedDataLayer(DatabaseSettings databaseSettings)
    {
        var connectionString = $"mongodb+srv://{databaseSettings.User}:{databaseSettings.Password}@{databaseSettings.Cluster}.mongodb.net/{databaseSettings.Name}?w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseSettings.Name);
        _feedCollection = database.GetCollection<FeedEntity>("feeds");
    }

    public async Task<Feed?> GetFeed(string guid)
    {
        var filter = Builders<FeedEntity>.Filter.Eq(p => p.Guid, guid);
        var person = await _feedCollection.Find(filter).FirstOrDefaultAsync();
        if (person != null)
        {
            return person.ToDomain();
        }
        await _feedCollection.InsertOneAsync(new FeedEntity());
        person = await _feedCollection.Find(filter).FirstOrDefaultAsync();
        return person?.ToDomain();
    }

    public async Task<IReadOnlyList<Feed>> GetAllActiveFeeds()
    {
        var filter = Builders<FeedEntity>.Filter.Eq(f => f.IsActive, true);
        var feedEntities = await _feedCollection.Find(filter).ToListAsync();
        return feedEntities.Select(f => f.ToDomain()).ToList();
    }

    public async Task<bool> InsertFeed(Feed feed)
    {
        try
        {
            await _feedCollection.InsertOneAsync(new FeedEntity
            {
                ChannelId = feed.ChannelId,
                GuildId = feed.GuildId,
                LastPostDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                CreatedUserId = feed.CreatedUserId,
                Guid = feed.Guid,
                IsActive = feed.IsActive,
                MessageTemplate = feed.MessageTemplate,
                LastUpdatedUserId = feed.LastUpdatedUserId,
                Url = feed.Url,
            });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    //public async Task<bool> SetTimeZone(string userId, string timeZone)
    //{
    //    var person = await GetPerson(userId);
    //    if (person == null)
    //    {
    //        return false;
    //    }
    //    var filter = Builders<PersonEntity>.Filter.Eq(p => p.UserId, userId);
    //    var update = Builders<PersonEntity>.Update.Set(p => p.TimeZone, timeZone);
    //    var updateResult = await _feedCollection.UpdateOneAsync(filter, update);
    //    return updateResult.MatchedCount == 1;
    //}

    //public async Task<bool> SetBirthday(string userId, string birthday)
    //{
    //    var person = await GetPerson(userId);
    //    if (person == null)
    //    {
    //        return false;
    //    }
    //    var filter = Builders<PersonEntity>.Filter.Eq(p => p.UserId, userId);
    //    var update = Builders<PersonEntity>.Update.Set(p => p.Birthday, birthday);
    //    var updateResult = await _feedCollection.UpdateOneAsync(filter, update);
    //    return updateResult.MatchedCount == 1;
    //}
}
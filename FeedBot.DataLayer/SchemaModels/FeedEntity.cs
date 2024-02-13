using FeedBot.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;
using NodaTime.Text;

namespace FeedBot.DataLayer.SchemaModels;

[BsonIgnoreExtraElements]
public class FeedEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("feedGuid")]
    public string Guid { get; set; }

    [BsonElement("url")]
    public string Url { get; set; }

    [BsonElement("guildId")]
    public string GuildId { get; set; }

    [BsonElement("channelId")]
    public string ChannelId { get; set; }

    [BsonElement("createdUserId")]
    public string CreatedUserId { get; set; }

    [BsonElement("lastUpdatedUserId")]
    public string LastUpdatedUserId { get; set; }

    [BsonElement("lastPostDate")]
    public string LastPostDate { get; set; }

    [BsonElement("messageTemplate")]
    public string MessageTemplate { get; set; }
    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    public Feed ToDomain()
    {
        LocalDateTime? lastPostDate = LocalDateTimePattern.GeneralIso.Parse(LastPostDate).GetValueOrThrow();
        return new Feed(Guid, Url, ChannelId, GuildId, lastPostDate, MessageTemplate, CreatedUserId, LastUpdatedUserId, IsActive);
    }
}
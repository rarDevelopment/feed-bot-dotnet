using NodaTime;

namespace FeedBot.Models;

public class Feed(
    string guid,
    string url,
    string channelId,
    string guildId,
    LocalDateTime? lastPostDate,
    string messageTemplate,
    string createdUserId,
    string lastUpdatedUserId,
    bool isActive)
{
    public string Guid { get; set; } = guid;
    public string Url { get; set; } = url;
    public string ChannelId { get; set; } = channelId;
    public string GuildId { get; set; } = guildId;
    public LocalDateTime? LastPostDate { get; set; } = lastPostDate;
    public string MessageTemplate { get; set; } = messageTemplate;
    public string CreatedUserId { get; set; } = createdUserId;
    public string LastUpdatedUserId { get; set; } = lastUpdatedUserId;
    public bool IsActive { get; set; } = isActive;
}
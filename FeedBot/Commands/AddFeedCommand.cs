using DiscordDotNetUtilities.Interfaces;
using FeedBot.DataLayer;
using FeedBot.Models;
using NodaTime;

namespace FeedBot.Commands;

public class AddFeedCommand(
    IDiscordFormatter discordFormatter,
    IFeedDataLayer feedDataLayer,
    IFeedProcessor feedProcessor,
    IClock clock)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add-feed", "Add a feed to this channel.")]
    public async Task AddFeedSlashCommand(
        [Summary("feed-url", "The URL for the RSS feed")] string feedUrl)
    {
        await DeferAsync();
        var currentUtcTime = clock.GetCurrentInstant().InUtc();

        var isValidFeed = feedProcessor.IsValidFeed(feedUrl);
        if (!isValidFeed)
        {
            await FollowupAsync(embed: discordFormatter.BuildErrorEmbed("Error Adding Feed",
            "Could not process this feed. Are you sure it's a proper RSS feed?"));
            return;
        }

        var rssFeed = feedProcessor.ProcessFeed(feedUrl);

        var feed = new Feed(Guid.NewGuid().ToString(),
            feedUrl,
            Context.Channel.Id.ToString(),
            Context.Guild.Id.ToString(),
            currentUtcTime.LocalDateTime,
            "",
            Context.User.Id.ToString(),
            Context.User.Id.ToString(),
            true);

        var success = await feedDataLayer.InsertFeed(feed);
        if (success)
        {
            await FollowupAsync(embed: discordFormatter.BuildRegularEmbedWithUserFooter("Added Feed",
                $"Successfully added the feed **{rssFeed.Title}**",
                Context.User));
        }
        else
        {
            await FollowupAsync(embed: discordFormatter.BuildErrorEmbed("Error Adding Feed",
                "These was an error adding this feed."));
        }
    }
}
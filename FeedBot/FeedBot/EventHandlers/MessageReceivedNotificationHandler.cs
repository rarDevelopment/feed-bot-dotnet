using FeedBot.Notifications;
using MediatR;

namespace FeedBot.EventHandlers;

public class MessageReceivedNotificationHandler : INotificationHandler<MessageReceivedNotification>
{
    public Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            await notification.Message.AddReactionAsync(new Emoji("😇"));
            return Task.CompletedTask;
        });
        return Task.CompletedTask;
    }
}
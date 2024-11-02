using Application.UseCases.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.NotificationHandlers;

public class EmailHandler : INotificationHandler<BookAddedNotification>
{
    private readonly ILogger<EmailHandler> _logger;
    public EmailHandler(ILogger<EmailHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(BookAddedNotification notification, CancellationToken cancellationToken)
    {
        //Email Sending implementations should be here...

        _logger.LogInformation($"Book name={notification.book.Name} is added and sent Email by Email Notification Handler.");
        return Task.CompletedTask;
    }
}

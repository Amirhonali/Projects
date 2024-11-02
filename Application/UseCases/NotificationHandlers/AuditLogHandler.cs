using Application.UseCases.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.NotificationHandlers;

public class AuditLogHandler : INotificationHandler<BookAddedNotification>
{
    private readonly ILogger<EmailHandler> _logger;
    public AuditLogHandler(ILogger<EmailHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(BookAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Audit logs added");
        return Task.CompletedTask;
    }
}

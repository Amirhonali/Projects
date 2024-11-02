using Domain.Entities;
using MediatR;

namespace Application.UseCases.Notifications;

public class BookAddedNotification : INotification
{
    public Domain.Entities.Book book { get; set; }
}

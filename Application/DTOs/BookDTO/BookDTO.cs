using Domain.Enums;

namespace Application.DTOs.BookDTO;

public class BookGetDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string ISBN { get; set; }

    public string? Description { get; set; }

    public DateOnly PublishDate { get; set; }

    public int[] AuthorsId { get; set; }

    public BookCategory Category { get; set; }
}

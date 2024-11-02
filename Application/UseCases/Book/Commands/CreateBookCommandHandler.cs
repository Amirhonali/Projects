using System;
using System.ComponentModel.DataAnnotations;
using Application.DTOs.BookDTO;
using Application.Models;
using Application.Repositories;
using Application.UseCases.Notifications;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Book.Commands
{
	public class CreateBookCommand : IRequest<ResponseCore<CreateBookCommandResult>>
	{
        [Required(ErrorMessage = "Name is required field")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20!")]
        public string Name { get; set; }

        [StringLength(8, ErrorMessage = "ISBN lenght must be 8!", MinimumLength = 8)]
        public string ISBN { get; set; }

        public string? Description { get; set; }

        public DateTime PublishDate { get; set; }

        public int[] AuthorsId { get; set; }

        public BookCategory Category { get; set; }
    }
	public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ResponseCore<CreateBookCommandResult>>
	{
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<Domain.Entities.Book> _validator;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(IBookRepository bookRepository, IValidator<Domain.Entities.Book> validator, IAuthorRepository authorRepository, IMediator mediator, IMapper mapper)
		{
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _validator = validator;
            _mediator = mediator;
            _mapper = mapper;
		}

        public async Task<ResponseCore<CreateBookCommandResult>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var result = new ResponseCore<CreateBookCommandResult>();

            BookAddedNotification bookNotification = new()
            {
                book = new Domain.Entities.Book() { Name = "test" }
            };

            await _mediator.Publish(bookNotification);

            Domain.Entities.Book book = _mapper.Map<Domain.Entities.Book>(request);
            var validationRes = _validator.Validate(book);

            if (!validationRes.IsValid)
            {
                result.ErrorMessege = validationRes.Errors.ToArray();
                result.StatusCode = 400;
                return result;
            }
            
            List<Author> authors = new();
            for (int i = 0; i < book.Authors.Count; i++)
            {
                Author author = book.Authors.ToArray()[i];
                author = await _authorRepository.GetByIdAsync(author.Id);
                if (author == null)
                {
                    result.ErrorMessege = new string[] { "Author Id:" + author.Id + "Not Found!" };
                    result.StatusCode = 400;
                    return result;
                }
                authors.Add(author);
            }
            book.Authors = authors;
            book = _bookRepository.AddAsync(book).Result;
            if (book == null)
            {
                result.ErrorMessege = new string[] { "Book not found!" };
                result.StatusCode = 400;
                return result;
            }

            result.Result = _mapper.Map<CreateBookCommandResult>(book);
            result.StatusCode = 200;
            return result;
        }
    }

	public class CreateBookCommandResult
	{
        public int Id { get; set; }
        public string Name { get; set; }

        public string ISBN { get; set; }

        public string? Description { get; set; } 

        public DateOnly PublishDate { get; set; }

        public int[] AuthorsId { get; set; }

        public BookCategory Category { get; set; }
    }
}


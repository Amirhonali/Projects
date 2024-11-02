using System;
using Application.Models;
using Application.Repositories;
using AutoMapper;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Book.Commands
{
	public class UpdateBookCommand : IRequest<ResponseCore<UpdateBookCommandResult>>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ISBN { get; set; }

        public string? Description { get; set; }

        public DateTime PublishDate { get; set; }

        //public int[] AuthorsId { get; set; }

        public BookCategory Category { get; set; }
    }

    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, ResponseCore<UpdateBookCommandResult>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<Domain.Entities.Book> _validator;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandler(IBookRepository bookRepository, IValidator<Domain.Entities.Book> validator, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _validator = validator;
            _mapper = mapper;
        }   

        public Task<ResponseCore<UpdateBookCommandResult>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var result = new ResponseCore<UpdateBookCommandResult>();
            Domain.Entities.Book book = _mapper.Map<Domain.Entities.Book>(request);
            var validationRes = _validator.Validate(book);
            if (!validationRes.IsValid)
            {
                result.ErrorMessege = validationRes.Errors.ToArray();
                result.StatusCode = 400;
                return Task.FromResult(result);
            }

            book = _bookRepository.UpdateAsync(book).Result;
            if (book == null)
            {
                result.ErrorMessege = new string[] { "Book Not Found!!!" };
                result.StatusCode = 400;
                return Task.FromResult(result);
            }
            return Task.FromResult(result);
        }
    }

    public class UpdateBookCommandResult
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


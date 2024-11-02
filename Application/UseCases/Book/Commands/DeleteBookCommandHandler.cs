using System;
using Application.Models;
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Book.Commands
{
	public class DeleteBookCommand: IRequest<ResponseCore<DeleteBookCommandResult>>
	{
        public int BookId { get; set; }
    }

    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, ResponseCore<DeleteBookCommandResult>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public DeleteBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Task<ResponseCore<DeleteBookCommandResult>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var result = new ResponseCore<DeleteBookCommandResult>(); 
            if(!_bookRepository.DeleteAsync(request.BookId).Result)
            {
                result.ErrorMessege = new string[] { "Book not found!" };
                result.StatusCode = 400;
                return Task.FromResult(result);
            }
            result.StatusCode = 200;
            return Task.FromResult(result);
        }
    }

    public class DeleteBookCommandResult
    {

    }
}


using System;
using Application.Repositories;
using AutoMapper;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Book.Queries
{
	public class GetBookByIdQuery
	{
		
	}

	public class GetBookByIdQueryHandler : IRequestHandler<>
	{
		private readonly IBookRepository _bookRepository;
		private readonly IMapper _mapper;

        public GetBookByIdQueryHandler(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }


    }

	public class GetBookByIdQueryResult
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


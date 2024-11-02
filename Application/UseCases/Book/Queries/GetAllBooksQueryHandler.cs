using System;
using Application.DTOs.BookDTO;
using Application.Models;
using Application.Repositories;
using AutoMapper;
using Domain.Enums;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.UseCases.Book.Queries
{
	public class GetAllBooksQuery:IRequest<ResponseCore<IEnumerable<GetAllBooksQueryResult>>>
	{

	}

    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, ResponseCore<IEnumerable<GetAllBooksQueryResult>>>
    {
        private readonly IAppCache _lazyCache;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private const string _Key = "MyLazyCache";

        public GetAllBooksQueryHandler(IAppCache lazyCache, IBookRepository bookRepository, IMapper mapper)
        {
            _lazyCache = lazyCache;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<ResponseCore<IEnumerable<GetAllBooksQueryResult>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var result = new ResponseCore<GetAllBooksQueryResult>();
            IEnumerable<GetAllBooksQueryResult> res = await _lazyCache.GetOrAdd(_Key,
               async options =>
               {
                   options.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                   options.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                   IEnumerable<Domain.Entities.Book> books = _bookRepository.Get(x => true);

                   IEnumerable<GetAllBooksQueryResult> booksRes = _mapper.Map<IEnumerable<GetAllBooksQueryResult>>(books);
                   return booksRes;

               });
            Console.WriteLine("GetAllBooks");
            return Ok(res);

        }
    }

    public class GetAllBooksQueryResult
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


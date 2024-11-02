using Application.DTOs.BookDTO;
using Application.Repositories;
using Application.UseCases.Book.Commands;
using Application.UseCases.Book.Queries;
using Application.UseCases.Notifications;
using AutoMapper;
using BookCatalogApi.Filters;
using Domain.Entities;
using FluentValidation;
using LazyCache;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BookCatalogApi.Controllers
{
    [Route("api/[controller]")]
    public class BookController : ApiControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<Book> _validator;
        private readonly IMediator _mediator;
        private readonly IAppCache _lazyCache;
        private const string _Key = "MyLazyCache";
        public BookController(IBookRepository bookRepository, IValidator<Book> validator, IAuthorRepository authorRepository, IAppCache lazyCache, IMediator mediator)
        {
            _bookRepository = bookRepository;
            _validator = validator;
            _authorRepository = authorRepository;
            _lazyCache = lazyCache;
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        // [ResponseCache(Duration = 20)]
        //  [CustomAuthorizationFilter("GetAllBooks")]

        [EnableCors("PolicyForMicrosoft")]
        public async Task<IActionResult> GetAllBooks()
        {
            #region Caching
            //bool IsActive = _lazyCache.TryGetValue(_Key, out IEnumerable<BookGetDTO> CacheBooks);
            //if (!IsActive)
            //{
            //    var books = await _bookRepository.GetAsync(x => true);

            //    IEnumerable<BookGetDTO> booksRes = _mapper.Map<IEnumerable<BookGetDTO>>(books);

            //    var entryOptions = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
            //        .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            //    _lazyCache.Add(_Key, booksRes, entryOptions);
            //    Console.WriteLine(" _lazyCache hit ");
            //    return Ok(booksRes);
            //}
            //return Ok(CacheBooks);
            #endregion
            var result = _mediator.Send(new GetBookByIdQuery() { BookId = id}).Result;
            return result.StatusCode >= 400 ? BadRequest(result) : Ok(result);
        }


        [HttpGet("[action]/{id}")]
        [CustomAuthorizationFilter("GetBook")]

        public async Task<IActionResult> GetBookById(int id)
        {
            var result = _mediator.Send(new GetAllBooksQuery()).Result;
            return result.StatusCode >= 400 ? BadRequest(result) : Ok(result);
        }

        #region CreateBookWithoutMediatr
        /*
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookCreate)
        {
            BookAddedNotification bookNotification = new()
            {
                book = new Book() { Name = "test" }
            };

            await _mediator.Publish(bookNotification);

            Book book = _mapper.Map<Book>(bookCreate);
            var validationRes = _validator.Validate(book);

            if (!validationRes.IsValid)
                return BadRequest(validationRes);
            List<Author> authors = new();
            for (int i = 0; i < book.Authors.Count; i++)
            {
                Author author = book.Authors.ToArray()[i];
                author = await _authorRepository.GetByIdAsync(author.Id);
                if (author == null)
                {
                    return NotFound("Author Id:" + author.Id + "Not found!");
                }
                authors.Add(author);
            }
            book.Authors = authors;
            book = await _bookRepository.AddAsync(book);
            if (book == null) NotFound("Book not found!");
          
            

            return Ok(_mapper.Map<BookGetDTO>(book));


        }
        */
        #endregion

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand bookCreate)
        {
            var result = await _mediator.Send(bookCreate);
            return result.StatusCode >= 400 ? BadRequest(result) : Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookCommand updateBook)
        {
            var result = await _mediator.Send(updateBook);  
            return result.StatusCode >= 400 ? BadRequest(result) : Ok(result);


        }
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteBook([FromQuery] int bookId)
        {
            var result = await _mediator.Send(new DeleteBookCommand() { BookId = bookId });
            return result.StatusCode >= 400 ? BadRequest(result) : Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult SearchBook(string text)
        {
            return Ok(_bookRepository.SearchBooks(text));
        }
    }
}

using Application.DTOs.AuthorDTO;
using Application.Repositories;
using BookCatalogApi.Filters;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace BookCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("PolicyForPDP")]
    public class AuthorController : ApiControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<Author> _validator;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _cache;

        private readonly string _Cache_Key = "MyKey";
        public AuthorController(IBookRepository bookRepository, IAuthorRepository authorRepository, IValidator<Author> validator, IMemoryCache memoryCache, IDistributedCache cache)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _validator = validator;
            _memoryCache = memoryCache;
            _cache = cache;
        }

        [HttpGet("[action]")]
        [CustomAuthorizationFilter("GetAuthor")]

        public async Task<IActionResult> GetAuthorById([FromQuery] int id)
        {

            if (_memoryCache.TryGetValue(id.ToString(), out AuthorGetDTO CachedAuthor))
            {
                return Ok(CachedAuthor);
            }
            Author author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound($"Author Id:{id} not found!");
            }
            AuthorGetDTO authorGet = _mapper.Map<AuthorGetDTO>(author);
            _memoryCache.Set(id.ToString(), authorGet);
            return Ok(authorGet);
        }

        [HttpGet("[action]")]
        [OutputCache(Duration = 30)]
        [CacheResourceFilter("AllAuthorsCache")]
        //[CustomAuthorizationFilter("GetAllAuthors")]
        public async Task<IActionResult> GetAllAuthors()
        {
           // string? CachedAuthors = await _cache.GetStringAsync(_Cache_Key);

            //if (string.IsNullOrEmpty(CachedAuthors))
            //{
                IEnumerable<Author> Authors = _authorRepository.Get(x => true);
                IEnumerable<AuthorGetDTO> resAuthors = _mapper.Map<IEnumerable<AuthorGetDTO>>(Authors);
                //await _cache.SetStringAsync(_Cache_Key, JsonSerializer.Serialize(resAuthors), new DistributedCacheEntryOptions
                //{
                //    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
                //});
               return Ok(resAuthors);
            //}
            //else
            //{
            //    Console.WriteLine("GetStringAsync return json");
            //    var res = JsonSerializer.Deserialize<IEnumerable<AuthorGetDTO>>(CachedAuthors);
            //    return Ok(res);
            //}


        }

        [HttpPost("[action]")]
        [CustomAuthorizationFilter("CreateAuthor")]

        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDTO createDTO)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            Author author = _mapper.Map<Author>(createDTO);
            var validresult = _validator.Validate(author);

            if (!validresult.IsValid)
                return BadRequest(validresult);

            //for (int i = 0; i < author.Books.Count; i++)
            //{
            //    Book book = author.Books.ToArray()[i];

            //    book = await _bookRepository.GetByIdAsync(book.Id);
            //    if (book == null)
            //    {
            //        return NotFound($"Book id not found");
            //    }
            //}
            author = await _authorRepository.AddAsync(author);
            if (author == null) return NotFound();
            AuthorGetDTO authorGet = _mapper.Map<AuthorGetDTO>(author);
            _memoryCache.Remove(_Cache_Key);
            return Ok(authorGet);


        }

        [HttpPut("[action]")]
        [Authorize(Roles = "UpdateAuthor")]

        public async Task<IActionResult> UpdateAuthor([FromBody] AuthorUpdateDTO createDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Author author = _mapper.Map<Author>(createDTO);
            var validationRes = _validator.Validate(author);

            if (!validationRes.IsValid)
                return BadRequest(validationRes);
            author = await _authorRepository.UpdateAsync(author);
            if (author == null) return NotFound();
            AuthorGetDTO authorGet = _mapper.Map<AuthorGetDTO>(author);
            _memoryCache.Remove(authorGet.Id);
            _memoryCache.Remove(_Cache_Key);
            return Ok(authorGet);


        }

        [HttpDelete("[action]")]
        [Authorize(Roles = "DeleteAuthor")]

        public async Task<IActionResult> DeleteAuthor([FromQuery] int id)
        {
            bool isDelete = await _authorRepository.DeleteAsync(id);
            _memoryCache.Remove(id);
            _memoryCache.Remove(_Cache_Key);
            return isDelete ? Ok("Deleted successfully")
                : BadRequest("Delete operation failed");
        }
        [HttpPost("[action]")]
        [EnableCors]
        public async Task<IActionResult> post()
        {
            return Ok();
        }
    }
}

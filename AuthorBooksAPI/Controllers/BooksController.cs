using AuthorBooksAPI.Application.DTO;
using AuthorBooksAPI.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthorBooksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;

        public BooksController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            var books = await _booksRepository.GetAllAsync();

            var result = books.Select(b => new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                BookNumber = b.BookNumber,
                PublishedYear = b.PublishedYear,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                DeletedDate = b.DeletedDate
            }).ToList();

            return Ok(result);
        }
        
    }
}

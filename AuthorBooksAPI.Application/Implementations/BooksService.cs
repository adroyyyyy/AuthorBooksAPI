using AuthorBooksAPI.Application.DTO;
using AuthorBooksAPI.Application.Interfaces;
using AuthorBooksAPI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorBooksAPI.Application.Implementations
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;

        public BooksService(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
           var books = await _booksRepository.GetAllAsync();
           return books.Select(x => new BookDto
           {
               BookId = x.BookId,
               Title = x.Title,
               BookNumber = x.BookNumber,
               PublishedYear = x.PublishedYear
           }).ToList();

        }
    }
}

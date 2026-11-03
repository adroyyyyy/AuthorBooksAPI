using AuthorBooksAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorBooksAPI.Domain.DTO
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string BookNumber { get; set; }
        public int PublishedYear { get; set; }
    }
}

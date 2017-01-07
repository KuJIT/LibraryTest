using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MvcApplication5.Models
{
    public class Book
    {
        public int BookId { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public byte[] Image { get; set; }

        public string ImageMIMEtype { get; set; }
    }


    public class TakeBook
    {
        public int TakeBookId { get; set; }

        public int BookId { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }
    }

    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<TakeBook> TakeBooks { get; set; }

    }

  //  public class 
}
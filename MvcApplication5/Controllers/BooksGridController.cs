using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication5.Models;
using System.IO;
using System.Web.Security;

namespace MvcApplication5.Controllers
{
    public class BooksGridController : Controller
    {
        BookContext db = new BookContext();

        public JsonResult getBooks(int page, int start, int limit, string filterField = null, string filterText = null, 
            string onlyInStock = null)
        {
            filterField = filterField == "" ? null : filterField;
            filterText = filterText == "" ? null : filterText;
            onlyInStock = onlyInStock == "" ? null : onlyInStock;


            var resultAllBooks = (from b in db.Books
                          select new
                          {
                              BookId = b.BookId,
                              author = b.Author,
                              title = b.Title,
                              //image = b.Image
                          });
            if (filterText != null)
            {
                switch (filterField)
                {
                    case "All":
                    case null:
                        resultAllBooks = resultAllBooks.Where(b => b.author.Contains(filterText) ||
                            b.title.Contains(filterText));
                    break;
                    case "Author":
                        resultAllBooks = resultAllBooks.Where(b => b.author.Contains(filterText));
                    break;
                    case "Title":
                        resultAllBooks = resultAllBooks.Where(b => b.title.Contains(filterText));
                    break;
                }

            }

            var res = (
                from b in resultAllBooks
                join t in db.TakeBooks 
                on b.BookId equals t.BookId
                into temp
                from j in temp.DefaultIfEmpty()
                select new
                {
                    BookId = b.BookId,
                    author = b.author,
                    title = b.title,
                    //image = b.image,
                    takeDate = (DateTime?)j.Date
                });

            if (onlyInStock != null)
                res = res.Where(r => r.takeDate == null);

            var resArr = res.OrderBy(b=>b.BookId).Skip(start).Take(limit).ToArray();
            var resFin = resArr.Select(b => new
            {
                BookId = b.BookId,
                author = b.author,
                title = b.title,
               // image = Convert.ToBase64String(b.image),
                takeDate = b.takeDate == null? "В наличии": ((DateTime)b.takeDate).ToString("d")
            }).ToArray();
            var dbugres = Json(new { success = true, total = res.Count(), books = resFin }, JsonRequestBehavior.AllowGet);
            return dbugres; 
        }

        public JsonResult GetMyBook()
        {
            if (Membership.GetUser() == null)
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            Guid userId = (Guid)Membership.GetUser().ProviderUserKey;

            var allMyBooks = db.TakeBooks.Where(t => t.UserId == userId).Select(
                t => new { date = t.Date, bookId = t.BookId });
            var resArr = allMyBooks.Join(db.Books, t => t.bookId, b => b.BookId, (t, b) =>
                new
                {
                    BookId = b.BookId,
                    author = b.Author,
                    title = b.Title,
                   // image = b.Image,
                    takeDate = t.date
                }).ToArray();


            var resFin = resArr.Select(b => new
            {
                BookId = b.BookId,
                author = b.author,
                title = b.title,
               // image = Convert.ToBase64String(b.image),
                takeDate = b.takeDate.AddDays(10).ToString("d")
            }).ToArray();

            return Json(new { success = true, books = resFin }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[Authorize(Roles="Admin")]
        public JsonResult AddBook(Book book, HttpPostedFileBase uploadImage)
        {
            byte[] imageData = null;
            

            using (var binaryReader = new BinaryReader(uploadImage.InputStream))
            {
                imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
            }
            book.ImageMIMEtype = uploadImage.ContentType;
            book.Image = imageData;
            db.Books.Add(book);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult TakeBook(string BookId = null)
        {
            string errorText = null;
            if (Membership.GetUser() == null)
                errorText = "Сначала войдите в систему";

            int bookId;
            if (!Int32.TryParse(BookId, out bookId))
                errorText = "Некорректный запрос";
            else if (db.Books.Find(bookId) == null)
                errorText = "Книги с таким ID не существует";
            if (db.TakeBooks.Where(b => b.BookId == bookId).Count() != 0)
                errorText = "Этой книги нет в наличии";

            if (errorText != null)
                return Json(new { success = false, errorText = errorText }, JsonRequestBehavior.AllowGet);


            TakeBook takeBook = new TakeBook {BookId = bookId, Date = DateTime.Now, 
                UserId = (Guid)Membership.GetUser().ProviderUserKey};
            db.TakeBooks.Add(takeBook);
            db.SaveChanges();
            return Json(new { success = true, errorText = "Всё ОК" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnBook(string BookId = null)
        {
            string errorText = null;
            if (Membership.GetUser() == null)
                errorText = "Сначала войдите в систему";

            int bookId;
            Guid userId = (Guid)Membership.GetUser().ProviderUserKey;
            if (!Int32.TryParse(BookId, out bookId))
                errorText = "Некорректный запрос";
            else if (db.TakeBooks.Where(t => t.BookId == bookId && t.UserId == userId).Count() == 0)
                errorText = "Вы не брали эту книгу";
            

            if (errorText != null)
                return Json(new { success = false, errorText = errorText }, JsonRequestBehavior.AllowGet);

            TakeBook TakeForRemove = db.TakeBooks.Where(t => t.BookId == bookId && t.UserId == userId).FirstOrDefault();

            db.TakeBooks.Remove(TakeForRemove);
            db.SaveChanges();
            return Json(new { success = true, errorText = "Всё ОК" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BookSearch(string filterField = null, string text = null)
        {
            if (text == null || text == "")
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var booksQ = db.Books.Select(b => new { BookId = b.BookId, title = b.Title, author = b.Author, text = text});

            switch (filterField)
                {
                    case "":
                    case "All":
                    case null:
                        booksQ = booksQ.Where(b => b.author.StartsWith(text) ||
                            b.title.StartsWith(text)).Select(b => new 
                            {
                                BookId = b.BookId, 
                                title = b.title, 
                                author = b.author, 
                                text = b.author.StartsWith(text)?b.author:b.title
                            });
                    break;
                    case "Author":
                    booksQ = booksQ.Where(b => b.author.StartsWith(text)).Select(b => new
                    {
                        BookId = b.BookId,
                        title = b.title,
                        author = b.author,
                        text =  b.author
                    });
                    break;
                    case "Title":
                    booksQ = booksQ.Where(b => b.title.StartsWith(text)).Select(b => new
                    {
                        BookId = b.BookId,
                        title = b.title,
                        author = b.author,
                        text = b.title
                    });
                    break;
                }

            var books = booksQ.Take(5).ToArray();
            return Json(new { success = true, books = books }, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult getImageByID(string BookId = null)
        {
            string errorText = null;
            int bookId;
            bool idIsInt = Int32.TryParse(BookId, out bookId);
            if (BookId == null || BookId == "" || !idIsInt)
                errorText = "ID некорректный";
            Book myBook = db.Books.Find(bookId);
            if (myBook == null)
                errorText = "Книги с таким ID не существует";
            if (errorText != null)
                return null;
            return File(myBook.Image, myBook.ImageMIMEtype);
        }

        public JsonResult deleteBook(string BookId = null)
        {
            string errorText = null;
            int bookId;
            bool idIsInt = Int32.TryParse(BookId, out bookId);
            if (BookId == null || BookId == "" || !idIsInt)
                errorText = "Некорректный ID";
            Book book = db.Books.Find(bookId);
            if (book == null)
                errorText = "Книги с таким ID не существует";
            if (db.TakeBooks.Where(t => t.BookId == bookId).Count() != 0)
                errorText = "Эта книга сейчас не в библиотеке. Сначала её нужно вернуть";

            if (errorText != null)
                return Json(new { success = false, errorText = errorText }, JsonRequestBehavior.AllowGet);
            db.Books.Remove(book);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
       
        //[Authorize(Roles = "Admin")]
        public JsonResult getDebtorsBooks()
        {
            var debtorsBooks = db.TakeBooks.Join(db.Books, t => t.BookId, b => b.BookId, (t, b) => 
                new 
                { 
                    userId = t.UserId, 
                    takeDate = t.Date, 
                    title = b.Title, 
                    author = b.Author,
                    BookId = b.BookId
                }).ToArray();

            var debtorsBooksForm = debtorsBooks.Select(d =>
                new
                {
                    user = Membership.GetUser(d.userId).UserName,
                    takeDate = d.takeDate.AddDays(10).ToString("d"),
                    title = d.title,
                    author = d.author,
                    BookId = d.BookId
                }).ToArray();

            return Json(new { success = true, books = debtorsBooksForm }, JsonRequestBehavior.AllowGet);
        }

    }
}

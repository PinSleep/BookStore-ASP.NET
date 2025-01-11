using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using System.Linq;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly LiteDBContext _context;

        public BookController(LiteDBContext context)
        {
            _context = context;
        }

        
        public IActionResult Index(string searchTerm)
        {
            
            var books = _context.Books.FindAll();

            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchTerm)
                                      || b.Author.ToLower().Contains(searchTerm));
            }

            
            return View(books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Insert(book);
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            var book = _context.Books.FindById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public IActionResult Delete(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            var book = _context.Books.FindById(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Delete(id);
            return RedirectToAction("Index");
        }
    }
}

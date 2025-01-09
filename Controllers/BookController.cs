using Microsoft.AspNetCore.Mvc;
using BookStore.Data;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly LiteDBContext _context;

        // Dependency Injection dla LiteDBContext
        public BookController(LiteDBContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            // Sprawdzanie, czy użytkownik jest administratorem
            return TempData["IsAdmin"] != null && (bool)TempData["IsAdmin"];
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Insert(book); // Wstawienie nowej książki do bazy.
                return RedirectToAction("Index");
            }

            return View(book);
        }

        public IActionResult Index()
        {
            var books = _context.Books.FindAll(); // Pobieranie książek
            return View(books); // Zwrócenie widoku z książkami
        }

        // Akcja edytowania książki
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            var book = _context.Books.FindById(id); // Znalezienie książki po ID
            if (book == null)
            {
                return NotFound();
            }
            return View(book); // Zwrócenie widoku edycji z książką
        }

        // Akcja zapisywania zmian w książce
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Update(book); // Aktualizowanie książki w bazie
                return RedirectToAction("Index"); // Po zapisaniu przekierowanie na stronę główną
            }
            return View(book);
        }

        // Akcja usuwania książki
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            var book = _context.Books.FindById(id); // Znalezienie książki po ID
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Delete(id); // Usunięcie książki z bazy
            return RedirectToAction("Index"); // Po usunięciu przekierowanie na stronę główną
        }
    }
}

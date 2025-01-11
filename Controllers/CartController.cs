using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly LiteDBContext _dbContext;

        public CartController(LiteDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Dodaje książkę do koszyka (w bazie LiteDB)
        [HttpGet]
        public IActionResult Add(int bookId)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return Json(new { success = false, message = "Not allowed." });
            }

            var username = User.Identity.Name;
            var user = _dbContext.Users.FindOne(u => u.Username == username);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var item = new CartItem
            {
                UserId = user.Id,
                BookId = bookId
            };
            _dbContext.CartItems.Insert(item);

            return Json(new { success = true, message = "Book added to cart." });
        }

        // Usuwa książkę z koszyka
        [HttpGet]
        public IActionResult Remove(int bookId)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            var username = User.Identity.Name;
            var user = _dbContext.Users.FindOne(u => u.Username == username);
            if (user == null)
            {
                return BadRequest();
            }

            var item = _dbContext.CartItems.FindOne(ci => ci.UserId == user.Id && ci.BookId == bookId);
            if (item != null)
            {
                _dbContext.CartItems.Delete(item.Id);
            }

            var cartBooks = GetCartBooks(user.Id);
            return PartialView("_CartPartial", cartBooks);
        }

        // Zwraca partial koszyka
        [HttpGet]
        public IActionResult GetCartPartial()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return Content("<p>Your cart is empty.</p>");
            }

            var username = User.Identity.Name;
            var user = _dbContext.Users.FindOne(u => u.Username == username);
            if (user == null)
            {
                return Content("<p>Your cart is empty.</p>");
            }

            var cartBooks = GetCartBooks(user.Id);
            return PartialView("_CartPartial", cartBooks);
        }

        // GET: /Cart/Shipping
        [HttpGet]
        public IActionResult Shipping()
        {
            // Tylko dla zalogowanego nie-admina
            if (!User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Book");
            }

            return View(new ShippingViewModel());
        }

        // POST: /Cart/Shipping
        [HttpPost]
        public IActionResult Shipping(ShippingViewModel model)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Book");
            }

            // Sprawdzamy walidację
            if (!ModelState.IsValid)
            {
                // Jeśli brakuje któregoś z wymaganych pól (np. BuildingNumber)
                return View(model);
            }

            // Jeśli wszystkie pola są ok, wyświetlamy popup
            ViewBag.OrderSuccess = true;

            // (opcjonalnie) można wyczyścić koszyk tutaj
            // ...

            return View(model);
        }

        private List<Book> GetCartBooks(int userId)
        {
            var items = _dbContext.CartItems.Find(ci => ci.UserId == userId).ToList();
            var bookIds = items.Select(ci => ci.BookId).ToHashSet();
            var books = _dbContext.Books.Find(b => bookIds.Contains(b.Id)).ToList();
            return books;
        }
    }
}

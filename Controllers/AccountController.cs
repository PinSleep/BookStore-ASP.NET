using BookStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly LiteDBContext _dbContext;

        public AccountController(LiteDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Widok logowania
        public IActionResult Login()
        {
            return View();
        }

        // Akcja logowania
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _dbContext.Users.FindOne(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["Username"] = user.Username;
                TempData["IsAdmin"] = user.IsAdmin;
                TempData.Keep();

                // Dodanie ciasteczka uwierzytelniającego
                HttpContext.Response.Cookies.Append("Username", user.Username);
                return RedirectToAction("Index", "Book");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // Widok rejestracji
        public IActionResult Register()
        {
            return View();
        }

        // Akcja rejestracji
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (_dbContext.Users.Exists(u => u.Username == username))
            {
                ViewBag.Error = "User already exists.";
                return View();
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                IsAdmin = false // Domyślnie użytkownik nie jest adminem
            };

            _dbContext.Users.Insert(newUser);

            TempData["Username"] = username;
            TempData["IsAdmin"] = newUser.IsAdmin;
            TempData.Keep();

            HttpContext.Response.Cookies.Append("Username", username);
            return RedirectToAction("Index", "Book");
        }

        // Akcja wylogowania
        public IActionResult Logout()
        {
            TempData.Clear();
            HttpContext.Response.Cookies.Delete("Username");
            return RedirectToAction("Index", "Book");
        }
    }
}

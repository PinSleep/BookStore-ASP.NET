using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        // Prosta baza użytkowników w pamięci
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

        // Lista użytkowników administracyjnych
        private static readonly HashSet<string> AdminUsers = new HashSet<string> { "admin" };

        // Widok logowania
        public IActionResult Login()
        {
            return View();
        }

        // Akcja logowania
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (Users.TryGetValue(username, out string storedPassword) && storedPassword == password)
            {
                TempData["Username"] = username;
                TempData["IsAdmin"] = AdminUsers.Contains(username);
                TempData.Keep();
                return RedirectToAction("Index", "Home");
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
            if (Users.ContainsKey(username))
            {
                ViewBag.Error = "User already exists.";
                return View();
            }

            // Dodajemy użytkownika do bazy
            Users[username] = password;

            TempData["Username"] = username;
            TempData["IsAdmin"] = AdminUsers.Contains(username);
            TempData.Keep();

            return RedirectToAction("Index", "Book");
        }

        // Akcja wylogowania
        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Index", "Book");
        }
    }
}

using BookStore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            
            var user = _dbContext.Users.FindOne(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                };

                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                
                return RedirectToAction("Index", "Book");
            }

            
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        
        public IActionResult Register()
        {
            
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
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
                IsAdmin = false 
            };
            _dbContext.Users.Insert(newUser);

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newUser.Username),
                new Claim(ClaimTypes.Role, newUser.IsAdmin ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Book");
        }

        // Akcja wylogowania
        public async Task<IActionResult> Logout()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Book");
        }

        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

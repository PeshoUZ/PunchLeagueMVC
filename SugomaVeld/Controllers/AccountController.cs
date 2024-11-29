using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using SugomaVeld.Data;
using SugomaVeld.Models;

namespace YourApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var passwordHash = HashPassword(Password);
            var user = _db.Users.SingleOrDefault(u => u.Email == Email && u.PasswordHash == passwordHash);

            if (user != null)
            {
                // Store the user ID in Session
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Home", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = Email,
                    PasswordHash = HashPassword(Password)
                };

                _db.Users.Add(user);
                _db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View();
        }
        public IActionResult Logout()
        {
            // Clear the session when logging out
            HttpContext.Session.Clear();

            // Redirect to the home page or login page after logout
            return RedirectToAction("Home", "Home");
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
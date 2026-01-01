using MessManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly MessDbContext _context;
        private readonly IConfiguration _configuration;

        // Inject Configuration to read the Secret Key
        public AccountController(MessDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // ------------------------------------------
                // 1. GENERATE JWT TOKEN
                // ------------------------------------------
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FullName", user.FullName)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2), // Token expires in 2 hours
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // ------------------------------------------
                // 2. STORE TOKEN IN COOKIE
                // ------------------------------------------
                Response.Cookies.Append("jwt_token", tokenString, new CookieOptions
                {
                    HttpOnly = true, // JavaScript cannot access it (Secure)
                    Secure = true,   // Only works on HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                // ------------------------------------------
                // 3. SET SESSION (For compatibility)
                // ------------------------------------------
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserRole", user.Role);

                // ---------------------------------------------------------
                // 4. CHECK: FORCE PASSWORD CHANGE ON FIRST LOGIN
                // ---------------------------------------------------------
                if (user.IsFirstLogin == true)
                {
                    return RedirectToAction("ChangePassword");
                }

                // ---------------------------------------------------------
                // 5. REDIRECT TO DASHBOARD
                // ---------------------------------------------------------
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Dashboard", "Teacher");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public IActionResult Logout()
        {
            // Delete the JWT Cookie
            Response.Cookies.Delete("jwt_token");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ---------------------------------------------------------
        // FEATURE: FORCE PASSWORD CHANGE
        // ---------------------------------------------------------

        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Security Check: User must be logged in (Session exists)
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.Password = newPassword;
                user.IsFirstLogin = false; // MARK AS COMPLETE
                _context.SaveChanges();
            }

            // Redirect to their respective dashboard
            if (user.Role == "Admin") return RedirectToAction("Dashboard", "Admin");
            else return RedirectToAction("Dashboard", "Teacher");
        }
    }
}
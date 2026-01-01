using MessManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Needed for Include

namespace MessManagementSystem.Controllers
{
    public class TeacherController : Controller
    {
        private readonly MessDbContext _context;

        public TeacherController(MessDbContext context)
        {
            _context = context;
        }

        // Helper: Get the ID of the currently logged-in teacher
        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // 1. Teacher Dashboard
        public IActionResult Dashboard()
        {
            int userId = GetUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId);
            return View(user);
        }

        // 2. View My Attendance History
        public IActionResult MyAttendance()
        {
            int userId = GetUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // Get attendance records for this user
            var history = _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AttendanceDate)
                .ToList();

            return View(history);
        }

        // 3. View My Bills
        public IActionResult MyBills()
        {
            int userId = GetUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var bills = _context.Bills
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BillYear)
                .ThenByDescending(b => b.BillMonth)
                .ToList();

            return View(bills);
        }
    }
}
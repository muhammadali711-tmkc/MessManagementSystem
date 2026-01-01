using MessManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for .Include()

namespace MessManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly MessDbContext _context;

        public AdminController(MessDbContext context)
        {
            _context = context;
        }

        // 1. Security Check
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // 2. The Dashboard Page
        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TotalTeachers = _context.Users.Count(u => u.Role == "Teacher");
            ViewBag.UnpaidBills = _context.Bills.Count(b => b.IsPaid == false);

            return View();
        }

        // ---------------------------------------------------------
        // FEATURE: MANAGE TEACHERS
        // ---------------------------------------------------------

        public IActionResult ManageTeachers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var teachers = _context.Users
                .Where(u => u.Role == "Teacher")
                .OrderBy(u => u.FullName)
                .ToList();

            return View(teachers);
        }

        [HttpGet]
        public IActionResult AddTeacher()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }


        [HttpPost]
        public IActionResult AddTeacher(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "This Email is already registered!";
                return View(user);
            }

            user.Role = "Teacher";
            user.IsFirstLogin = true;
            user.CurrentBalance = 0;
            user.IsActive = true;

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("ManageTeachers");
        }
        // ---------------------------------------------------------
        // FEATURE: EDIT & DELETE TEACHERS
        // ---------------------------------------------------------

        // 1. Show Edit Page
        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var teacher = _context.Users.Find(id);
            if (teacher == null) return RedirectToAction("ManageTeachers");

            return View(teacher);
        }

        // 2. Save Changes
        [HttpPost]
        public IActionResult EditTeacher(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var existingTeacher = _context.Users.Find(user.UserId);
            if (existingTeacher != null)
            {
                existingTeacher.FullName = user.FullName;
                existingTeacher.Email = user.Email;
                // We do not change Password or Balance here to keep it safe

                _context.SaveChanges();
            }
            return RedirectToAction("ManageTeachers");
        }

        // 3. Delete Teacher (And all their history)
        public IActionResult DeleteTeacher(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var teacher = _context.Users.Find(id);
            if (teacher != null)
            {
                // We must delete dependent data first to avoid SQL Foreign Key Errors!

                // A. Delete their Payments
                var payments = _context.Payments.Where(p => p.UserId == id);
                _context.Payments.RemoveRange(payments);

                // B. Delete their Bills
                var bills = _context.Bills.Where(b => b.UserId == id);
                _context.Bills.RemoveRange(bills);

                // C. Delete their Attendance
                var attendance = _context.Attendances.Where(a => a.UserId == id);
                _context.Attendances.RemoveRange(attendance);

                // D. Finally, Delete the User
                _context.Users.Remove(teacher);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageTeachers");
        }
        // ---------------------------------------------------------
        // FEATURE: MANAGE MENU
        // ---------------------------------------------------------

        public IActionResult ManageMenu()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var today = DateOnly.FromDateTime(DateTime.Today);

            var menus = _context.DailyMenus
                .Where(m => m.MenuDate >= today)
                .OrderBy(m => m.MenuDate)
                .ToList();

            return View(menus);
        }

        [HttpGet]
        public IActionResult AddMenu()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult AddMenu(DailyMenu menu)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (_context.DailyMenus.Any(m => m.MenuDate == menu.MenuDate))
            {
                ViewBag.Error = "A menu for this date already exists!";
                return View(menu);
            }

            _context.DailyMenus.Add(menu);
            _context.SaveChanges();

            return RedirectToAction("ManageMenu");
        }

        public IActionResult DeleteMenu(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var menu = _context.DailyMenus.Find(id);
            if (menu != null)
            {
                _context.DailyMenus.Remove(menu);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageMenu");
        }

        // ---------------------------------------------------------
        // FEATURE: MARK ATTENDANCE
        // ---------------------------------------------------------

        [HttpGet]
        public IActionResult Attendance()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var todayDate = DateOnly.FromDateTime(DateTime.Today);

            // 1. Check if Menu exists for today
            var todaysMenu = _context.DailyMenus.FirstOrDefault(m => m.MenuDate == todayDate);
            if (todaysMenu == null)
            {
                ViewBag.Error = "No Menu found for today! Please add a Menu first.";
                return View(new List<User>());
            }

            ViewBag.MenuInfo = $"{todaysMenu.LunchItem} + {todaysMenu.DinnerItem} (Rs. {todaysMenu.DailyRate})";

            // 2. Get all Teachers
            var teachers = _context.Users
                .Where(u => u.Role == "Teacher")
                .ToList();

            // 3. Get existing attendance for today
            var existingAttendance = _context.Attendances
                .Where(a => a.AttendanceDate == todayDate && a.IsPresent == true && a.UserId != null)
                .Select(a => (int)a.UserId) // Force convert to int
                .ToList();

            ViewBag.PresentUserIds = existingAttendance;

            return View(teachers);
        }

        [HttpPost]
        public IActionResult SaveAttendance(int[] selectedIds)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var todayDate = DateOnly.FromDateTime(DateTime.Today);

            // 1. Clear old records for today
            var oldRecords = _context.Attendances.Where(a => a.AttendanceDate == todayDate);
            _context.Attendances.RemoveRange(oldRecords);
            _context.SaveChanges();

            var allTeachers = _context.Users.Where(u => u.Role == "Teacher").ToList();

            foreach (var teacher in allTeachers)
            {
                var isPresent = selectedIds.Contains(teacher.UserId);

                var att = new Attendance
                {
                    UserId = teacher.UserId,
                    AttendanceDate = todayDate,
                    IsPresent = isPresent,
                    RecordedAt = DateTime.Now
                };

                _context.Attendances.Add(att);
            }

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // ---------------------------------------------------------
        // FEATURE: GENERATE MONTHLY BILLS
        // ---------------------------------------------------------

        // 1. Show the "Generate Bills" Page
        [HttpGet]
        public IActionResult GenerateBills()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // 2. The Logic to Calculate and Save
        [HttpPost]
        public IActionResult ProcessBills(int month, int year)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Check if bills already exist
            bool billsExist = _context.Bills.Any(b => b.BillMonth == month && b.BillYear == year);
            if (billsExist)
            {
                ViewBag.Error = $"Bills for {month}/{year} have already been generated!";
                return View("GenerateBills");
            }

            // A. Get Water Bill Rate
            var waterConfig = _context.SystemConfigs.FirstOrDefault(c => c.KeyName == "MonthlyWaterBill");
            decimal waterRate = waterConfig != null ? waterConfig.Value : 500;

            // B. Get all Teachers
            var teachers = _context.Users.Where(u => u.Role == "Teacher").ToList();
            int billsGeneratedCount = 0;

            foreach (var teacher in teachers)
            {
                // C. Calculate Food Cost
                var foodCost = (from a in _context.Attendances
                                join m in _context.DailyMenus on a.AttendanceDate equals m.MenuDate
                                where a.UserId == teacher.UserId
                                      && a.IsPresent == true
                                      && a.AttendanceDate.Month == month
                                      && a.AttendanceDate.Year == year
                                select m.DailyRate).Sum();

                // D. Handle Null CurrentBalance
                decimal previousDues = teacher.CurrentBalance ?? 0;

                // E. Create the Bill
                var bill = new Bill
                {
                    UserId = teacher.UserId,
                    BillMonth = month,
                    BillYear = year,
                    FoodAmount = foodCost,
                    WaterAmount = waterRate,
                    PreviousArrears = previousDues,
                    TotalAmount = foodCost + waterRate + previousDues,
                    IsPaid = false,
                    GeneratedDate = DateTime.Now
                };

                _context.Bills.Add(bill);

                // F. Update Teacher's Total Debt
                teacher.CurrentBalance = (teacher.CurrentBalance ?? 0) + (foodCost + waterRate);

                billsGeneratedCount++;
            }

            _context.SaveChanges();

            TempData["Success"] = $"Successfully generated bills for {billsGeneratedCount} teachers!";
            return RedirectToAction("GenerateBills");
        }

        // 3. View Bill History
        public IActionResult BillHistory()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var bills = _context.Bills
                .Include(b => b.User)
                .OrderByDescending(b => b.BillYear)
                .ThenByDescending(b => b.BillMonth)
                .ToList();

            return View(bills);
        }
    }
}
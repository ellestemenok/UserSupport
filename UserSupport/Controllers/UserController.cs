using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UserSupport.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context; // DATABAZA
        }

        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            var username = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            User user = await _context.Users.Include(u => u.Tutorials).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.UserRole = role;
            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UnverifiedList()
        {
            List<User> unverifiedUsers = await _context.Users.Where(u => !u.IsVerified).ToListAsync();
            return View(unverifiedUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsVerified = true;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("UnverifiedList");
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteUser(int id, string returnUrl)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(returnUrl);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AssignRoles()
        {
            var username = User.Identity.Name;
            List<User> users = await _context.Users.Where(u => u.IsVerified && u.Username != username).ToListAsync();
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UpdateUserRole(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = role;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssignRoles");
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }
    }
}

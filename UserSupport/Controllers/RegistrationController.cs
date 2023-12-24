using UserSupport.Infrastructure;

namespace UserSupport.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _context;
        public RegistrationController(AuthService userService, AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] User user, IFormFile ProfileImage)
        {       
            if (ModelState.IsValid)
            {
                user.IsVerified = false;
                user.Role = "Читатель";
                user.Tutorials = new List<Tutorial>();
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await ProfileImage.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        user.ProfileImage = Convert.ToBase64String(fileBytes);
                    }
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Auth");
            }
            ModelState.AddModelError("", "Не все поля заполнены");
            return View(user);
        }
    }
}

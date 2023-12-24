using UserSupport.Infrastructure;

namespace UserSupport.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _userService;
        private readonly AppDbContext _context;
        public AuthController(AuthService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Authenticate(User user)
        {
            var userLogin = _userService.Authenticate(user.Username, user.Password);
            if (userLogin != null && userLogin.IsVerified)
            {
                var token = _userService.GenerateJwtToken(userLogin);
                HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(1),
                    HttpOnly = true,
                    Secure = true
                });
                return RedirectToAction("UserProfile", "User");
            }
            else if (userLogin != null && !userLogin.IsVerified)
            {
                ModelState.AddModelError("", "Администратор еще не подтвердил вашу учетную запись");
            }
            else
            {
                ModelState.AddModelError("", "Неверное имя пользователя или пароль");
            }
            return View("Login", user);
        }
    }
}

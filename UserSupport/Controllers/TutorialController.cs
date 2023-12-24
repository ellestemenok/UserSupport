using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UserSupport.Controllers
{
    [Authorize]
    public class TutorialController : Controller
    {
        private readonly AppDbContext _context;

        public TutorialController(AppDbContext context)
        {
            _context = context; // DATABAZA
        }
        public async Task<IActionResult> Index()
        {
            var tutorials = await _context.Tutorials.ToListAsync();
            return View(tutorials);
        }

        public async Task<IActionResult> Detail(int tutorialID)
        {
            var tutorial = await _context.Tutorials.SingleOrDefaultAsync(t => t.Id == tutorialID);
            if (tutorial == null)
            {
                return NotFound();
            }
            return View(tutorial);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор,Технический писатель")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Технический писатель")]
        public async Task<IActionResult> Create([FromForm] Tutorial tutorial, IFormFile CoverImage)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    // Обработка ошибки, если пользователь не найден
                    return NotFound("Пользователь не найден");
                }
                tutorial.UserId = user.Id;
                await ImgToBase64(tutorial, CoverImage);
                tutorial.CreatedAt = DateTime.Now;

                _context.Tutorials.Add(tutorial);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(tutorial);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор,Технический писатель")]
        public async Task<IActionResult> Edit(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null)
            {
                return NotFound();
            }
            return View(tutorial);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Технический писатель")]
        public async Task<IActionResult> Edit([FromForm] int id, Tutorial tutorial, IFormFile CoverImage)
        {
            if (id != tutorial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    var username = User.Identity.Name;
                    var user = _context.Users.FirstOrDefault(u => u.Username == username);
                    tutorial.UserId = user.Id;
                    ImgToBase64(tutorial, CoverImage);
                    tutorial.CreatedAt = DateTime.Now;
                    _context.Update(tutorial);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
            }
            return View(tutorial);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Технический писатель")]
        public async Task<IActionResult> Delete(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null)
            {
                return NotFound();
            }

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserProfile", "User");
        }
        public async Task ImgToBase64(Tutorial tutorial, IFormFile coverImage)
        {
            if (coverImage != null && coverImage.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await coverImage.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    tutorial.CoverImage = Convert.ToBase64String(fileBytes);
                }
            }
        }
    }
}

using AdminEventOrganizer.Interface;
using AdminEventOrganizer.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(userId))
            {
                switch (role)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Dashboard");
                    default:
                        HttpContext.Session.Clear();
                        break;
                }
            }

            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["ErrorMessage"] = "Email dan password wajib diisi!";
                    return View();
                }

                var user = await _userRepository.Login(email, password);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Email atau password salah!";
                    return View();
                }

                if (user.Role != "Admin")
                {
                    TempData["ErrorMessage"] = "Hanya admin yang dapat mengakses sistem ini!";
                    return View();
                }

                if (!user.IsActive)
                {
                    TempData["ErrorMessage"] = "Akun tidak aktif. Hubungi administrator.";
                    return View();
                }

                // Set session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Username", user.Name);

                _logger.LogInformation($"Admin login berhasil: {user.Email}");

                // Redirect ke returnUrl atau dashboard admin
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                TempData["SuccessMessage"] = $"Selamat datang, {user.Name}!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saat login untuk email: {email}");
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem. Silakan coba lagi.";
                return View();
            }
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");

                HttpContext.Session.Clear();

                _logger.LogInformation($"User logout: {username}");

                TempData["SuccessMessage"] = "Anda telah logout.";
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saat logout");
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet("logout")]
        public IActionResult LogoutGet()
        {
            return Logout();
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Cek apakah email sudah terdaftar
                var existingUser = await _userRepository.GetByEmail(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email sudah terdaftar.");
                    return View(model);
                }

                var newUser = await _userRepository.Create(model);

                TempData["SuccessMessage"] = "Akun User berhasil ditambahkan.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saat register untuk email: {model.Email}");
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem. Silakan coba lagi.";
                return View(model);
            }
        }
    }
}
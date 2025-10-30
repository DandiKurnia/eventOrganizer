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

        [HttpGet("register")]
        public IActionResult Register()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (!string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserModel model)
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

                // Register user baru
                var newUser = await _userRepository.Register(model);

                _logger.LogInformation($"User berhasil register: {model.Email}");

                TempData["SuccessMessage"] = "Registrasi berhasil! Silakan login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saat register untuk email: {model.Email}");
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem. Silakan coba lagi.";
                return View(model);
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (!string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Index", "Dashboard");
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

                // HANYA ADMIN yang boleh login
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
                HttpContext.Session.SetString("Username", user.Username);

                _logger.LogInformation($"Admin login berhasil: {user.Email}");

                // Redirect ke returnUrl atau dashboard admin
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                TempData["SuccessMessage"] = $"Selamat datang, {user.Username}!";
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
        [ValidateAntiForgeryToken] // Opsional untuk keamanan
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
    }
}
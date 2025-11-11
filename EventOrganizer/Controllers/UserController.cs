using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
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
            var adminId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(adminId))
            {
                var role = HttpContext.Session.GetString("Role");
                return RedirectToAction("Index", role);
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
                    TempData["ErrorMessage"] = "Email sudah terdaftar.";
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
            // Cek apakah user sudah login
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(userId))
            {
                // Jika sudah login, arahkan sesuai role-nya
                switch (role)
                {
                    case "Vendor":
                        return RedirectToAction("Index", "Vendor");
                    case "Customer":
                        return RedirectToAction("Index", "Customer");
                    default:
                        HttpContext.Session.Clear();
                        break;
                }
            }

            // Jika belum login, tampilkan halaman login
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

                if (!user.IsActive)
                {
                    TempData["ErrorMessage"] = "Akun tidak aktif. Hubungi administrator.";
                    return View();
                }

                // Simpan sesi login
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Username", user.Name);

                _logger.LogInformation($"Login berhasil: {user.Email} dengan role {user.Role}");

                // Redirect sesuai peran (role)
                switch (user.Role)
                {
                    case "Customer":
                        TempData["SuccessMessage"] = $"Selamat datang, {user.Name}!";
                        return RedirectToAction("Index", "Customer");

                    case "Vendor":
                        TempData["SuccessMessage"] = $"Selamat datang Vendor, {user.Name}!";
                        return RedirectToAction("Index", "Vendor");

                    default:
                        TempData["ErrorMessage"] = "Role tidak dikenal. Hubungi administrator.";
                        return View();
                }
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
    }
}

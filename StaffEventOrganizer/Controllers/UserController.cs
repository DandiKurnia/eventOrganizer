using Microsoft.AspNetCore.Mvc;
using StaffEventOrganizer.Interface;
using Models;

namespace StaffEventOrganizer.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUser _userRepository;

        public UserController(IUser userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
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
                    case "Staff":
                        return RedirectToAction("Index", "Staff");
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

                if (user.Role != "Staff")
                {
                    TempData["ErrorMessage"] = "Hanya staff yang dapat mengakses sistem ini!";
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

                TempData["SuccessMessage"] = $"Selamat datang Vendor, {user.Name}!";
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
    }
}

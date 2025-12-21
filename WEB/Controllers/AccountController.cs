using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;   // EF Context için
using EnerjiTahmin.Models; // Modeller için
using Microsoft.EntityFrameworkCore; 
using System.Text;
using System.Text.Json;
using EnerjiTahmin.DTOs;

namespace EnerjiTahmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context; // 🟢 EF Core Bağlantısı
        private readonly IHttpClientFactory _httpClientFactory; // 🟠 API Bağlantısı

        // Constructor'da ikisini de istiyoruz (Hibrit Mimari)
        public AccountController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // ==========================================
        // 1. GİRİŞ YAP (SOA / API KULLANIYOR)
        // ==========================================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // API'ye İstek Atıyoruz
            var client = _httpClientFactory.CreateClient("SoaApiClient");
            var content = new StringContent(JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Session Doldurma
                HttpContext.Session.SetString("UserEmail", data.user.email);
                HttpContext.Session.SetString("UserName", data.user.name);
                
                // DİKKAT: EF Core için ID lazım. API'den gelen ID'yi saklıyoruz.
                HttpContext.Session.SetString("UserID", data.user.id.ToString()); 
                
                string role = data.user.email.Contains("admin") ? "Admin" : "User";
                HttpContext.Session.SetString("UserRole", role);

                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.Error = "Giriş başarısız. E-posta veya şifre hatalı.";
            return View();
        }

        // ==========================================
        // 2. KAYIT OL (SOA / API KULLANIYOR)
        // ==========================================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Kullanici k)
        {
            var client = _httpClientFactory.CreateClient("SoaApiClient");
            var content = new StringContent(JsonSerializer.Serialize(new { name = k.AdSoyad, email = k.Email, password = k.Sifre }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("auth/register", content);

            if (response.IsSuccessStatusCode) return RedirectToAction("Login");
            
            ViewBag.Error = "Kayıt başarısız oldu.";
            return View(k);
        }

        // ==========================================
        // 3. ÇIKIŞ YAP (LOGOUT)
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================================
        // 4. PROFİLİM (PROFILE) 
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Profile() 
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            if (userIdString == null) return RedirectToAction("Login");
            
            var client = _httpClientFactory.CreateClient("SoaApiClient");
            
            
            var response = await client.GetAsync($"users/profile/{userIdString}"); 
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                
                
                var user = JsonSerializer.Deserialize<Kullanici>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(user);
            }
            
            
            return RedirectToAction("Logout"); 
        }

        [HttpPost]
        public async Task<IActionResult> Profile(Kullanici k) 
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            if (userIdString == null) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("SoaApiClient");
            
            
            var updateData = new 
            { 
                userId = userIdString,
                name = k.AdSoyad, 
                password = k.Sifre 
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateData), 
                Encoding.UTF8, 
                "application/json"
            );

            
            var response = await client.PutAsync($"users/update-profile", content); 

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("UserName", k.AdSoyad);
                ViewBag.Success = "Profil başarıyla güncellendi (SOA üzerinden)!";
                
                
                return View(k);
            }
            
            ViewBag.Error = "Profil güncelleme başarısız oldu.";
            return View(k);
        }

        // ==========================================
        // 5. ŞİFREMİ UNUTTUM 
        // ==========================================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email) 
        {
            var client = _httpClientFactory.CreateClient("SoaApiClient");
            var forgotData = new { email };
            var content = new StringContent(
                JsonSerializer.Serialize(forgotData), 
                Encoding.UTF8, 
                "application/json"
            );

            
            var response = await client.PostAsync("auth/forgot-password", content); 

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Yeni şifreniz e-posta adresinize gönderildi (SOA üzerinden)!";
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Error = errorResponse?.message ?? "İşlem hatası.";
            }

            return View();
        }

        // ==========================================
        // 6. HESABIMI SİL 
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> DeleteAccount() 
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            if (userIdString == null) return RedirectToAction("Login");
            
            var client = _httpClientFactory.CreateClient("SoaApiClient");

            
  
var response = await client.DeleteAsync($"auth/delete-account/{userIdString}");

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            
            
            ViewBag.Error = "Hesap silme işlemi başarısız oldu.";
            return RedirectToAction("Profile");
        }
    }
}
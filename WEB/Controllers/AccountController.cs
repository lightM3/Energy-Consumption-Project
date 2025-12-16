using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;   
using EnerjiTahmin.Models; 
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore; 
using EnerjiTahmin.Helpers; 
using System.Net.Http;
using System.Text;
using System.Text.Json; 
using EnerjiTahmin.DTOs; 

namespace EnerjiTahmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context; 
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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

            
            var response = await client.DeleteAsync($"users/delete-account/{userIdString}"); 

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
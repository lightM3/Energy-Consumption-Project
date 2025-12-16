using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;   // Veritabanı Context'i
using EnerjiTahmin.Models; // Kullanici Modeli
using Microsoft.AspNetCore.Http; // Session işlemleri
using Microsoft.EntityFrameworkCore; // ExecuteSqlRaw için gerekli
using EnerjiTahmin.Helpers; // MailHelper için

namespace EnerjiTahmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GİRİŞ YAP (LOGIN)
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Veritabanı kontrolü (EF Core ile okuma yapıyoruz)
            var user = _context.Kullanicilar
                        .FirstOrDefault(x => x.Email == email && x.Sifre == password);

            if (user != null)
            {
                // Giriş Başarılı -> Session Başlat
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Rol ?? "User"); // Null gelirse User ata
                HttpContext.Session.SetString("UserName", user.AdSoyad);

                // Yönlendirme Mantığı
                if (user.Rol == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Error = "E-posta veya şifre hatalı!";
                return View();
            }
        }

        // ==========================================
        // 2. KAYIT OL (REGISTER) - STORED PROCEDURE BURADA!
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Kullanici k)
        {
            // 1. Önce böyle bir mail var mı kontrol edelim (C# tarafında hızlı kontrol)
            var varMi = _context.Kullanicilar.Any(x => x.Email == k.Email);
            
            if (varMi)
            {
                ViewBag.Error = "Bu e-posta adresi zaten kayıtlı!";
                return View();
            }

            // 2. STORED PROCEDURE KULLANIMI
            // Veriyi doğrudan tabloya eklemek yerine, SQL'deki 'sp_AddUser' prosedürünü çağırıyoruz.
            // Bu yöntem Jürinin istediği yöntemdir.
            
            try
            {
                string varsayilanRol = "User"; // Yeni gelen herkes User olur

                // SQL: CALL sp_AddUser('Ad Soyad', 'Email', 'Sifre', 'Rol')
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_AddUser({0}, {1}, {2}, {3})", 
                    k.AdSoyad, 
                    k.Email, 
                    k.Sifre, 
                    varsayilanRol
                );

                // İşlem başarılıysa Login'e git
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // SQL'den hata dönerse (Örn: Rol bulunamadı) buraya düşer
                ViewBag.Error = "Kayıt sırasında hata oluştu: " + ex.Message;
                return View(k);
            }
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
        public IActionResult Profile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userEmail == null) return RedirectToAction("Login");

            var user = _context.Kullanicilar.FirstOrDefault(x => x.Email == userEmail);
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(Kullanici k)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var dbUser = _context.Kullanicilar.FirstOrDefault(x => x.Email == userEmail);

            if (dbUser != null)
            {
                dbUser.AdSoyad = k.AdSoyad;
                dbUser.Sifre = k.Sifre;
                
                _context.SaveChanges();

                HttpContext.Session.SetString("UserName", k.AdSoyad);
                
                ViewBag.Success = "Profil başarıyla güncellendi!";
                return View(dbUser);
            }

            return RedirectToAction("Login");
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
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Kullanicilar.FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                string yeniSifre = Guid.NewGuid().ToString().Substring(0, 6); 
                user.Sifre = yeniSifre;
                _context.SaveChanges();

                try
                {
                    MailHelper.MailGonder(user.Email, yeniSifre);
                    ViewBag.Success = "Yeni şifreniz e-posta adresinize gönderildi!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Mail hatası: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Error = "Bu e-posta adresi sistemde kayıtlı değil!";
            }

            return View();
        }

        // ==========================================
        // 6. HESABIMI SİL
        // ==========================================
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userEmail == null) return RedirectToAction("Login");

            var user = _context.Kullanicilar.FirstOrDefault(x => x.Email == userEmail);
            
            if (user != null)
            {
                _context.Kullanicilar.Remove(user);
                _context.SaveChanges();
                
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return RedirectToAction("Profile");
        }
    }
}
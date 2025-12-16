using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;
using EnerjiTahmin.Models;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // YETKİ KONTROLÜ İÇİN YARDIMCI METOT
    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "Admin";
    }

    // 1. DASHBOARD (ÖZET EKRANI)
    public IActionResult Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        // İstatistikleri Topla
        ViewBag.KullaniciSayisi = _context.Kullanicilar.Count();
        ViewBag.LogSayisi = _context.IslemLoglari.Count();
        
        // Veritabanındaki son 10 tüketim verisini grafiğe basmak için çekelim
        // (Eğer veri yoksa boş gelir, sorun değil)
        var sonVeriler = _context.TuketimVerileri
                                 .OrderByDescending(x => x.Tarih)
                                 .Take(10)
                                 .OrderBy(x => x.Tarih) // Grafikte tarih sırasıyla görünsün
                                 .ToList();

        return View(sonVeriler);
    }

    // 2. KULLANICI YÖNETİMİ
    public IActionResult Kullanicilar()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");


var users = _context.UserDetails.ToList();

        return View(users);
    }

    // KULLANICI SİLME
    public IActionResult KullaniciSil(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        var user = _context.Kullanicilar.Find(id);
        if (user != null)
        {
            _context.Kullanicilar.Remove(user);
            _context.SaveChanges();
        }
        return RedirectToAction("Kullanicilar");
    }

    // 3. LOG (GEÇMİŞ TAHMİNLER) İZLEME
    public IActionResult Loglar()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account");

        // Son 100 işlemi getir (Çok veri varsa sistemi yormasın)
        var logs = _context.IslemLoglari
                           .OrderByDescending(x => x.Tarih)
                           .Take(100)
                           .ToList();
        return View(logs);
    }
}
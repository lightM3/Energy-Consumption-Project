using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;
using EnerjiTahmin.Models;
using Microsoft.EntityFrameworkCore; // 🌟 ARTIK KESİN RENKLİ OLACAK!

namespace EnerjiTahmin.Controllers
{
    public class SuggestionController : Controller
    {
        private readonly AppDbContext _context;

        public SuggestionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Suggestion model)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (userIdStr == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                int userId = int.Parse(userIdStr);

                // 🔥 İŞTE BU SATIR RENGİ GETİRECEK! 🔥
                // 'AnyAsync' komutu sadece 'Microsoft.EntityFrameworkCore' içindedir.
                // Kullanıcı aynı konu başlığıyla daha önce mesaj atmış mı kontrol ediyoruz (Spam Koruması).
                bool ayniMesajVarMi = await _context.Oneriler
                                                    .AnyAsync(x => x.Konu == model.Konu && x.UserId == userId);

                if (ayniMesajVarMi)
                {
                    ViewBag.Error = "Bu konu başlığıyla zaten bir öneriniz var!";
                    return View(model);
                }

                // Ekleme işlemleri
                model.UserId = userId;
                model.Tarih = DateTime.Now;
                model.OkunduMu = false;

                await _context.Oneriler.AddAsync(model);
                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Öneriniz başarıyla iletildi!";
                return RedirectToAction("Create"); 
            }
            return View(model);
        }
    }
}
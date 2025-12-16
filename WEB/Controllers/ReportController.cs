using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Data;
using EnerjiTahmin.Models;
using Microsoft.EntityFrameworkCore; // FromSqlRaw için şart

namespace EnerjiTahmin.Controllers
{
public class ReportController : Controller
{
    private readonly AppDbContext _context;

    public ReportController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Rapor Sayfası
    public IActionResult Index(DateTime? baslangic, DateTime? bitis)
    {
        // 1. Varsayılan Değerler:
        // Eğer kullanıcı tarih seçmediyse, son 1 haftayı gösterelim.
        if (baslangic == null) baslangic = DateTime.Today.AddDays(-7);
        if (bitis == null) bitis = DateTime.Today;

        // 2. STORED PROCEDURE ÇAĞIRMA
        // Veri çeken bir SP olduğu için "FromSqlRaw" kullanıyoruz.
        // Gelen veriyi 'TuketimVerisi' modeline dönüştürüyoruz.
        
        var raporVerisi = _context.TuketimVerileri
            .FromSqlRaw("CALL sp_GetConsumptionByDateRange({0}, {1})", baslangic, bitis)
            .ToList();

        // Tarihleri View'a geri gönderelim ki inputların içinde seçili kalsın
        ViewBag.Baslangic = baslangic.Value.ToString("yyyy-MM-dd");
        ViewBag.Bitis = bitis.Value.ToString("yyyy-MM-dd");

        return View(raporVerisi);
    }
}
}

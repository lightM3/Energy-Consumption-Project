using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EnerjiTahmin.Models;
using EnerjiTahmin.Data;

namespace EnerjiTahmin.Controllers;

public class HomeController : Controller
{
     private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult CityStats()
    {
    var stats = _context.CityTemperatures.OrderByDescending(x => x.AvgTemp).ToList();
    
    return View(stats);
    }
    public IActionResult DailyStats()
{
    // Tarihe göre sondan başa sırala (En yeni gün en üstte)
    // Sadece son 30 günü getir ki liste çok uzamasın
    var data = _context.DailySummaries
                       .OrderByDescending(x => x.Date)
                       .Take(30) 
                       .ToList();

    return View(data);
}
public IActionResult Alerts()
{
    // En güncel yüksek tüketim uyarılarını en üstte getir
    var alerts = _context.HighConsumptions
                         .OrderByDescending(x => x.Date)
                         .ThenByDescending(x => x.Time)
                         .Take(50) // Sayfa şişmesin diye son 50 kayıt
                         .ToList();

    return View(alerts);
}

}

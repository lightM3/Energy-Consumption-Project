using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using EnerjiTahmin.Models; 
using EnerjiTahmin.Data;
using EnerjiTahmin.Helpers;

public class TahminController : Controller
{
    private readonly AppDbContext _context;
    private readonly EnerjiTahmin.Helpers.LogHelper _logger; 

    public TahminController(AppDbContext context, EnerjiTahmin.Helpers.LogHelper logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> VerileriHazirla([FromBody] TahminIstegiDto gelenVeri)
    {
        // 1. Verileri Hazırla
        int saatInt = 0;
        try { saatInt = int.Parse(gelenVeri.Saat.Split(':')[0]); } catch { }
        
        DateTime birGunOnce = gelenVeri.Tarih.AddDays(-1);

        // --- DÜZELTME YAPILAN YER ---
        // Veritabanı TimeSpan (14:00:00) tuttuğu için, ararken de TimeSpan'a çeviriyoruz.
        var arananSaat = new TimeSpan(saatInt, 0, 0);

        var gecmisVeri = _context.TuketimVerileri
                            .FirstOrDefault(x => x.Tarih == birGunOnce && x.Saat == arananSaat);
        // ----------------------------

        double lagDegeri = gecmisVeri != null ? gecmisVeri.TuketimMiktari : 0.0;
        
        var gonderilecekVeri = new
        {
            Tarih = gelenVeri.Tarih.ToString("yyyy-MM-dd"),
            Saat = saatInt,
            Sicaklik = gelenVeri.Sicaklik,
            Lag_24 = lagDegeri 
        };

        double tahminSonucu = 0;
        string mesaj = "";

        // 2. Python API'ye İstek At
        using (var client = new HttpClient())
        {
            try
            {
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(gonderilecekVeri), 
                    System.Text.Encoding.UTF8, 
                    "application/json");

                var response = await client.PostAsync("http://127.0.0.1:5000/predict", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(responseString))
                    {
                        if (doc.RootElement.TryGetProperty("prediction", out var predElement))
                        {
                            tahminSonucu = predElement.GetDouble();
                            mesaj = "AI Tahmini Başarılı (Prediction)";
                        }
                        else if (doc.RootElement.TryGetProperty("tahmin", out var tahminElement))
                        {
                            tahminSonucu = tahminElement.GetDouble();
                            mesaj = "AI Tahmini Başarılı (Tahmin)";
                        }
                        else
                        {
                            tahminSonucu = lagDegeri;
                            mesaj = "Python cevabı boş döndü.";
                        }
                    }
                }
                else
                {
                    mesaj = "Python Hatası (HTTP " + response.StatusCode + ")";
                    tahminSonucu = lagDegeri;
                }
            }
            catch (Exception ex)
            {
                mesaj = "Bağlantı Koptu: " + ex.Message;
                tahminSonucu = lagDegeri;
            }
        }

        try { _logger.LogOlustur("Tahmin", $"Sonuç: {tahminSonucu}"); } catch { }

        var sonucJson = new
        {
            tarih = gelenVeri.Tarih.ToString("yyyy-MM-dd"),
            saat = saatInt,
            sicaklik = gelenVeri.Sicaklik,
            lag_24h = lagDegeri,
            tahmin = tahminSonucu,
            debug_mesaj = mesaj
        };

        return Json(sonucJson);
    }
}

public class TahminIstegiDto
{
    public DateTime Tarih { get; set; }
    public string Saat { get; set; } = ""; 
    public double Sicaklik { get; set; }
}
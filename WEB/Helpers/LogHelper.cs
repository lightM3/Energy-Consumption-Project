using EnerjiTahmin.Data;
using EnerjiTahmin.Models;
using Microsoft.AspNetCore.Http; // Session ve IP için gerekli

namespace EnerjiTahmin.Helpers
{
    public class LogHelper
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogHelper(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // --- İŞTE EKSİK OLAN METOT BU ---
        public void LogOlustur(string islemTuru, string aciklama)
        {
            try
            {
                // Giriş yapan kullanıcının mailini al, yoksa "Anonim" yaz
                var email = _httpContextAccessor.HttpContext?.Session?.GetString("UserEmail") ?? "Anonim";
                
                // IP adresini al
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var log = new IslemLog
                {
                    KullaniciEmail = email,
                    IslemTuru = islemTuru,
                    Aciklama = aciklama,
                    IpAdresi = ip,
                    Tarih = DateTime.Now
                };

                _context.IslemLoglari.Add(log);
                _context.SaveChanges();
            }
            catch
            {
                // Log atarken hata olursa sistem çökmesin, sessizce devam etsin
            }
        }
    }
}
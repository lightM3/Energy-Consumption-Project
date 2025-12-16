using Microsoft.EntityFrameworkCore;
using EnerjiTahmin.Data;   // AppDbContext'in olduğu yer
using EnerjiTahmin.Models; // Modellerin olduğu yer
using System; // HttpClient için gerekebilir

var builder = WebApplication.CreateBuilder(args);

// 1. VERİTABANI BAĞLANTISI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🛑 A) SOA ENTEGRASYONU: HTTP CLIENT KAYDI
// Node.js SOA katmanına çağrı yapmak için HttpClientFactory kaydı yapılır.
builder.Services.AddHttpClient("SoaApiClient", client =>
{
    // Node.js SOA sunucusunun adresi (server.js dosyasından)
    client.BaseAddress = new Uri("http://localhost:5001/api/"); 
});
// 🛑 BİTİŞ

// 2. MVC VE SESSION SERVİSLERİ
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<EnerjiTahmin.Helpers.LogHelper>();

var app = builder.Build();

// --- OTOMATİK VERİ EKLEME (BAŞLANGIÇ) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // B) KULLANICI EKLEME (Login İçin)
    if (!context.Kullanicilar.Any())
    {
        // 1. Admin (Sen)
        context.Kullanicilar.Add(new Kullanici
        {
            AdSoyad = "Metin Serinkaya",
            Email = "admin@enerji.com",
            Sifre = "123", // NOT: SOA katmanı artık şifreyi hash'leyecektir, DB'ye direkt bu şifre gitmez. 
            Rol = "Admin"
        });

    }

    // Değişiklikleri kaydet
    context.SaveChanges();
}
// --- OTOMATİK VERİ EKLEME (BİTİŞ) ---

// --- HTTP AYARLARI ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization(); // Yetkilendirme
app.UseSession();       // Session Middleware'i (Sırası önemli!)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
namespace EnerjiTahmin.Models
{
    // 1. Bizim Göndereceğimiz (Request)
    public class NodeLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // 2. Node.js'ten Gelecek Cevap (Response)
    public class NodeLoginResponse
    {
        public bool Success { get; set; }      // İşlem başarılı mı?
        public string Message { get; set; }    // "Giriş başarılı" veya "Şifre yanlış"
        public string Token { get; set; }      // JWT Token
        public string Role { get; set; }       // "Admin" veya "User"
        
        // Kullanıcı Detayları (İç içe obje)
        public NodeUserInfo UserInfo { get; set; } 
    }

    // Kullanıcı detaylarının içi (Arkadaşına sor, ne dönüyorsa buraya ekle)
    public class NodeUserInfo
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }
}
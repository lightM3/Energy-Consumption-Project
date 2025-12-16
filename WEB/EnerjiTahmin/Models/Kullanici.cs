using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnerjiTahmin.Models
{
    [Table("users")]
    public class Kullanici
    {
        [Key]
        [Column("UserId")]
        public int Id { get; set; }

        [Column("FullName")]
        [Required]
        public string AdSoyad { get; set; } = ""; 

        [Column("Email")] // Veritabanında email sütun adı neyse onu yaz (Genelde 'Email'dir)
        [Required]
        public string Email { get; set; } = "";   

        [Column("PasswordHash")]
        [Required]
        public string Sifre { get; set; } = ""; 

        // ============================================================
        // BURASI ÇOK ÖNEMLİ: HEM SAYI HEM YAZI AYARI
        // ============================================================

        // 1. Veritabanındaki Gerçek Sütun (SAYI - Integer)
        
        [Column("RoleId")]
        public int RoleId { get; set; } 

        // 2. Senin Kodlarının Kullandığı "Sanal" Özellik (YAZI - String)
        [NotMapped] // Bu veritabanında yok, sadece kod içinde var
        public string Rol 
        { 
            get 
            {
                // Veritabanından gelen sayıyı yazıya çevirir
                return RoleId == 1 ? "Admin" : "User";
            }
            set 
            {
                // Senin kodundan gelen yazıyı sayıya çevirip veritabanına hazırlar
                if (value == "Admin") RoleId = 1;
                else RoleId = 2; // User veya diğerleri
            }
        }
    }
}
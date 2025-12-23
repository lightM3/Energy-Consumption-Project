using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnerjiTahmin.Models
{
    [Table("Suggestions")]
    public class Suggestion
    {
        [Key]
        public int Id { get; set; }

        // Hangi kullanıcı gönderdi? (İlişkisel Veritabanı Şovun)
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual Kullanici? GonderenKisi { get; set; }

        [Required(ErrorMessage = "Konu başlığı zorunludur.")]
        [StringLength(100)]
        public string Konu { get; set; }

        [Required(ErrorMessage = "Mesaj içeriği boş olamaz.")]
        public string Mesaj { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false; // Admin okuyunca işaretler
    }
}
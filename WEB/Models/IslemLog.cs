using System;
using System.ComponentModel.DataAnnotations;

namespace EnerjiTahmin.Models
{
    public class IslemLog
    {
        [Key]
        public int Id { get; set; }
        public string KullaniciEmail { get; set; }
        public string IslemTuru { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
        public string IpAdresi { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnerjiTahmin.Models
{
    [Table("electricityconsumptions")] // <-- TABLO ADI
    public class TuketimVerisi
    {
        [Key]
        [Column("ConsumptionID")] // <-- ID Sütunu
        public int Id { get; set; }

        [Column("Date")] // <-- Tarih Sütunu
        public DateTime Tarih { get; set; }

        [Column("Time")] // <-- Saat Sütunu
        public TimeSpan Saat { get; set; }

        [Column("MWh_Value")] // <-- Tüketim Değeri
        public double TuketimMiktari { get; set; }

        // Kodda kullanmıyoruz ama veritabanında var, dursun zararı olmaz
        [Column("Lag_24h")] 
        public decimal Lag24 { get; set; }
    }
}
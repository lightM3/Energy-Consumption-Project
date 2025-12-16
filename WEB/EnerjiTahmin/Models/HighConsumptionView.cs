using System;

namespace EnerjiTahmin.Models
{
    public class HighConsumptionView
    {
        public int ConsumptionID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }    // SQL'deki 'Time' tipi C#'ta TimeSpan olur
        public decimal MWh_Value { get; set; }
        public decimal? Lag_24h { get; set; } // Bir önceki günün verisi (Null gelebilir)
    }
}
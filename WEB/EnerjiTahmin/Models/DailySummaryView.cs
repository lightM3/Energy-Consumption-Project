using System;

namespace EnerjiTahmin.Models
{
    public class DailySummaryView
    {
        public DateTime Date { get; set; }
        public decimal TotalConsumption { get; set; } // SQL'deki SUM sonucu genelde decimal gelir
    }
}
namespace EnerjiTahmin.Models
{
    public class CityTempView
    {
        public string CityName { get; set; }
        public double AvgTemp { get; set; } // SQL'deki AVG sonucu ondalıklı döner
    }
}
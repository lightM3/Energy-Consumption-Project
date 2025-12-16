namespace EnerjiTahmin.Models
{
    // Veritabanındaki 'vw_userdetails' view'ının karşılığı
    public class UserDetailView
    {
        public int UserID { get; set; }
        public string MaskedName { get; set; }     
        public string MaskedEmail { get; set; }    
        public string MaskedPassword { get; set; } 
        public string RoleName { get; set; }
    }
}

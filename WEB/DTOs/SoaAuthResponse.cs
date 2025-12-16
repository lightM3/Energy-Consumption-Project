namespace EnerjiTahmin.DTOs
{
    
    public class SoaUser
    {
        
        public int id { get; set; } 
        public string? name { get; set; } 
    public string? email { get; set; } 
    }

    
    public class LoginResponse
    {
        public string? message { get; set; }
        public SoaUser? user { get; set; }
    }

    
    public class MessageResponse
    {
        public string? message { get; set; }
    }
}
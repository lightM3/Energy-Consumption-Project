using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace EnerjiTahmin.Controllers; 

public class ChatController : Controller
{
    [HttpPost]
    public async Task<IActionResult> GetResponse(string userMessage)
    {
        if (string.IsNullOrEmpty(userMessage)) return Json(new { success = false, message = "Boş mesaj." });

        try
        {
            using (var client = new HttpClient())
            {
              
                
                string apiKey = "AIzaSyC6SmQnIwXKKtxMJN8QzvJAn10kEQJCLwA"; 
                
              
string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = "Sen Türkçe konuşan bir asistansın. Soru: " + userMessage } } }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Yine hata verirse tam sebebini görelim
                    return Json(new { success = false, message = $"HATA: {response.StatusCode} - {responseString}" });
                }

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    if(doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var text = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                        return Json(new { success = true, reply = text });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Cevap yok." });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Sistem Hatası: " + ex.Message });
        }
    }
}
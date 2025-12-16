using System.Net;
using System.Net.Mail;
namespace EnerjiTahmin.Helpers;


public class MailHelper
{
    
        public static void MailGonder(string aliciEmail, string yeniSifre)
        {
            // 1. BURAYA KENDİ MAİLİNİ YAZ
            string gonderenEmail = "27metinserinkaya@gmail.com";
            
            // 2. BURAYA AZ ÖNCE ALDIĞIN YENİ 16 HANELİ ŞİFREYİ YAZ (Boşluksuz yaz)
            string gonderenSifre = "wgdk dfmd ejpf isip"; 

            try 
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gonderenEmail, "EnergySys Güvenlik");
                mail.To.Add(aliciEmail);
                mail.Subject = "Şifre Sıfırlama Talebi";
                mail.Body = $"<h1>Yeni Şifreniz: {yeniSifre}</h1><p>Lütfen giriş yapıp değiştirin.</p>";
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true; 
                
                // --- DÜZELTME BURADA: SIRALAMA ÖNEMLİ ---
                // Önce "Varsayılanı Kullanma" diyoruz
                smtp.UseDefaultCredentials = false; 
                
                // Sonra şifreyi veriyoruz (NetworkCredential en son atanmalı)
                smtp.Credentials = new NetworkCredential(gonderenEmail, gonderenSifre);
                
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Mail Hatası: " + ex.Message);
            }
        }
    }

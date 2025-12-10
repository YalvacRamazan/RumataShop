using System.Net;
using System.Net.Mail;

namespace RumataShop.Helpers
{
    public class MailHelper
    {
        // IConfiguration sayesinde appsettings.json dosyasını okuyabiliyoruz
        public static bool MailGonder(string aliciMail, string konu, string icerik, IConfiguration config)
        {
            try
            {
                // Ayarları JSON dosyasından çekiyoruz
                var sunucu = config["MailAyarlari:Sunucu"];
                var port = int.Parse(config["MailAyarlari:Port"]);
                var gonderenMail = config["MailAyarlari:GonderenMail"];
                var gonderenSifre = config["MailAyarlari:GonderenSifre"];

                SmtpClient smtp = new SmtpClient(sunucu, port);
                smtp.EnableSsl = true; // Gmail güvenli bağlantı ister
                smtp.Credentials = new NetworkCredential(gonderenMail, gonderenSifre);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gonderenMail, "Rumata Shop"); // Görünen isim
                mail.To.Add(aliciMail);
                mail.Subject = konu;
                mail.IsBodyHtml = true; // HTML tasarımı gönderebilelim diye
                mail.Body = icerik;

                smtp.Send(mail);
                return true; // Başarılı
            }
            catch (Exception ex)
            {
                // Hata olursa konsola yazdırabilirsin
                Console.WriteLine("Mail Hatası: " + ex.Message);
                return false; // Başarısız
            }
        }
    }
}
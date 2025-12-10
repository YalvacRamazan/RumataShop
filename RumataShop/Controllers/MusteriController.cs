using Microsoft.AspNetCore.Mvc;
using RumataShop.Models;
using RumataShop.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RumataShop.Helpers; // MailHelper için gerekli

namespace RumataShop.Controllers
{
    public class MusteriController : Controller
    {
        private readonly IMusteriRepository _musteriRepo;
        private readonly IConfiguration _config; // Appsettings okumak için

        public MusteriController(IMusteriRepository musteriRepo, IConfiguration config)
        {
            _musteriRepo = musteriRepo;
            _config = config;
        }

        // --- GİRİŞ SAYFASI (GET) ---
        [HttpGet]
        public IActionResult Giris()
        {
            return View();
        }

        // --- GİRİŞ İŞLEMİ (POST) ---
        [HttpPost]
        public async Task<IActionResult> Giris(string email, string sifre)
        {
            var musteri = await _musteriRepo.EmailIleGetir(email);

            if (musteri != null)
            {
                // Şifre Doğrulama
                bool sifreDogruMu = BCrypt.Net.BCrypt.Verify(sifre, musteri.MusteriSifreHash);

                if (sifreDogruMu)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, musteri.MusteriId.ToString()),
                        new Claim(ClaimTypes.Name, musteri.MusteriAd + " " + musteri.MusteriSoyad),
                        new Claim(ClaimTypes.Email, musteri.MusteriMail),
                        // Rol gelince burayı açarsın:
                         new Claim(ClaimTypes.Role, musteri.Rol)
                    };

                    var userIdentity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync("CookieAuth", principal);

                    // --- DEĞİŞİKLİK BURADA ---
                    // Eğer giren kişinin Rolü "Admin" ise -> Admin Paneline gönder
                    if (musteri.Rol == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    // Değilse -> Ana Sayfaya gönder
                    else
                    {
                        return RedirectToAction("Index", "Urun");
                    }
                }
            }

            ViewBag.Hata = "E-posta veya şifre hatalı!";
            return View();
        }

        // --- KAYIT İŞLEMİ (POST) ---
        [HttpPost]
        public async Task<IActionResult> Kayit(Musteri musteri)
        {
            ModelState.Remove("MusteriId");
            ModelState.Remove("MusteriKayitTarihi");
            ModelState.Remove("Rol");
            ModelState.Remove("AktifMi");

            if (ModelState.IsValid)
            {
                string hashliSifre = BCrypt.Net.BCrypt.HashPassword(musteri.MusteriSifreHash);
                musteri.MusteriSifreHash = hashliSifre;

                musteri.MusteriKayitTarihi = DateTime.Now;
                musteri.Rol = "Musteri";
                musteri.AktifMi = true;

                await _musteriRepo.Ekle(musteri);

                TempData["Mesaj"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Giris");
            }

            ViewBag.Hata = "Kayıt başarısız, lütfen bilgileri kontrol edin.";
            return View("Giris");
        }

        // --- ŞİFREMİ UNUTTUM (GERÇEK MAİL GÖNDERME) ---
        [HttpPost]
        public async Task<IActionResult> SifremiUnuttum(string email)
        {
            var musteri = await _musteriRepo.EmailIleGetir(email);

            if (musteri != null)
            {
                // 1. Token Oluştur
                string token = Guid.NewGuid().ToString();
                await _musteriRepo.TokenOlustur(musteri.MusteriId, token);

                // 2. Linki Hazırla
                string link = Url.Action("SifreYenile", "Musteri", new { token = token }, Request.Scheme);

                // 3. Mail İçeriği
                string mailIcerigi = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #7000ff;'>Rumata Shop - Şifre Yenileme</h2>
                        <p>Merhaba <strong>{musteri.MusteriAd}</strong>,</p>
                        <p>Hesabınız için şifre yenileme talebi aldık. Aşağıdaki butona tıklayarak yeni şifrenizi belirleyebilirsiniz:</p>
                        <br>
                        <a href='{link}' style='background-color: #7000ff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Şifremi Yenile</a>
                        <br><br>
                        <p style='color: #888; font-size: 12px;'>Bu işlemi siz yapmadıysanız, lütfen bu e-postayı dikkate almayın.</p>
                    </div>
                ";

                // 4. Maili Gönder (Helpers/MailHelper.cs kullanır)
                bool sonuc = MailHelper.MailGonder(email, "Şifre Sıfırlama Talebi", mailIcerigi, _config);

                if (sonuc)
                    TempData["Mesaj"] = "Sıfırlama bağlantısı e-posta adresinize gönderildi!";
                else
                {
                    ViewBag.Hata = "Mail gönderilemedi. Sunucu ayarlarını kontrol edin.";
                    return View("Giris");
                }
            }
            else
            {
                TempData["Mesaj"] = "Eğer kayıtlıysa, e-posta adresinize bağlantı gönderildi.";
            }

            return RedirectToAction("Giris");
        }

        // --- ŞİFRE YENİLEME SAYFASI (GET) ---
        [HttpGet]
        public IActionResult SifreYenile(string token)
        {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Giris");
            return View("SifreYenile", token); // Token'ı View'a model olarak atıyoruz
        }

        // --- ŞİFRE YENİLEME İŞLEMİ (POST) ---
        [HttpPost]
        public async Task<IActionResult> SifreYenile(string token, string yeniSifre)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(yeniSifre))
            {
                string hashliYeniSifre = BCrypt.Net.BCrypt.HashPassword(yeniSifre);
                await _musteriRepo.SifreYenile(token, hashliYeniSifre);

                TempData["Mesaj"] = "Şifreniz başarıyla güncellendi! Giriş yapabilirsiniz.";
                return RedirectToAction("Giris");
            }

            ViewBag.Hata = "Bir hata oluştu. Link geçersiz olabilir.";
            return View("SifreYenile", token);
        }

        // --- ÇIKIŞ İŞLEMİ ---
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Urun");
        }
    }
}
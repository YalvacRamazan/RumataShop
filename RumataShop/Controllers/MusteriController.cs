using Microsoft.AspNetCore.Mvc;
using RumataShop.Models;
using RumataShop.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace RumataShop.Controllers
{
    public class MusteriController : Controller
    {
        private readonly IMusteriRepository _musteriRepo;

        public MusteriController(IMusteriRepository musteriRepo)
        {
            _musteriRepo = musteriRepo;
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
            var musteri = await _musteriRepo.MusteriLogin(email, sifre);

            if (musteri != null)
            {
                // ARTIK DEFANSİF KODLARA GEREK YOK, ÇÜNKÜ SQL'DEN DOLU GELİYOR 🚀
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, musteri.MusteriId.ToString()),
                    new Claim(ClaimTypes.Name, musteri.MusteriAd + " " + musteri.MusteriSoyad),
                    new Claim(ClaimTypes.Email, musteri.MusteriMail) // Artık hata vermeyecek ✅
                };

                var userIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(userIdentity);

                // Tarayıcıya kimliği ver
                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index", "Urun");
            }

            ViewBag.Hata = "E-posta veya şifre hatalı!";
            return View();
        }

        // --- ÇIKIŞ İŞLEMİ (LOGOUT) ---
        [HttpGet]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Urun");
        }

        // --- KAYIT İŞLEMİ (POST) ---
        [HttpPost]
        public async Task<IActionResult> Kayit(string Ad, string Soyad, string Email, string Sifre)
        {
            var yeniMusteri = new Musteri
            {
                MusteriAd = Ad,
                MusteriSoyad = Soyad,
                MusteriMail = Email,
                MusteriSifreHash = Sifre,
                MusteriKayitTarihi = DateTime.Now // Tarih sorununu da çözmüştük
            };

            await _musteriRepo.MusteriKayit(yeniMusteri);

            ViewBag.Mesaj = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return View("Giris");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RumataShop.Models;
using RumataShop.Repositories;
using System.Security.Claims;

namespace RumataShop.Controllers
{
    // [Authorize]: Bu satır sayesinde giriş yapmamış kimse bu sayfaları göremez!
    // Otomatik olarak Login sayfasına atar.
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly ISiparisRepository _siparisRepo;

        public SiparisController(ISiparisRepository siparisRepo)
        {
            _siparisRepo = siparisRepo;
        }

        // 1. SİPARİŞLERİM / SEPETİM SAYFASI
        // SİPARİŞLERİM SAYFASI
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Giriş yapan kullanıcının ID'sini al
            var musteriIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(musteriIdString)) return RedirectToAction("Giris", "Musteri");

            int musteriId = int.Parse(musteriIdString);

            // Repository'den siparişleri çek
            // (MusteriSiparisleriniGetir metodu SQL'den sp_Musteri_Siparisleri_Getir'i çağırır)
            var siparisler = await _siparisRepo.MusteriSiparisleriniGetir(musteriId);

            return View(siparisler);
        }

        // 2. DETAY SAYFASI
        public async Task<IActionResult> Detay(int id)
        {
            var detaylar = await _siparisRepo.SiparisDetaylariniGetir(id);
            return View(detaylar);
        }

        // 3. SEPETE EKLEME (JavaScript Buraya İstek Atacak)
        [HttpPost]
        public async Task<IActionResult> SepeteEkle(int urunId, int adet = 1)
        {
            // Kim giriş yapmış?
            var musteriIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(musteriIdString))
            {
                // Giriş düşmüşse hata dön
                return Json(new { success = false, message = "Oturum süreniz dolmuş." });
            }

            int musteriId = int.Parse(musteriIdString);

            // SQL'e kaydet
            await _siparisRepo.SepeteEkle(musteriId, urunId, adet);

            // Başarılı mesajı dön
            return Json(new { success = true, message = "Ürün sepete eklendi!" });
        }
        [HttpGet]
        public async Task<IActionResult> SepetiGetir()
        {
            // 1. Giriş yapmış kullanıcının ID'sini al
            var musteriIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Giriş yapmamışsa boş liste döndür (Sepet boş görünür)
            if (string.IsNullOrEmpty(musteriIdString)) return Json(new List<SepetDetayDto>());

            int musteriId = int.Parse(musteriIdString);

            // 2. Veritabanından sepeti çek
            var sepetUrunleri = await _siparisRepo.SepetiGetir(musteriId);

            // 3. JSON olarak fırlat
            return Json(sepetUrunleri);
        }
        [HttpPost]
        public async Task<IActionResult> SepetGuncelle(int urunId, int degisim)
        {
            // 1. Giriş yapan kullanıcının ID'sini bul
            var musteriIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Güvenlik: Giriş yapmamışsa işlem yapma
            if (string.IsNullOrEmpty(musteriIdString)) return Json(new { success = false, message = "Oturum kapalı." });

            int musteriId = int.Parse(musteriIdString);

            // 2. Repository'e gönder (+1 veya -1)
            await _siparisRepo.SepetGuncelle(musteriId, urunId, degisim);

            // 3. Başarılı dön
            return Json(new { success = true });
        }
        // 1. JS'in Çağıracağı İşlem (POST)
        [HttpPost]
        public async Task<IActionResult> SiparisiTamamla()
        {
            var musteriIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(musteriIdString)) return Json(new { success = false, message = "Oturum kapalı." });

            int musteriId = int.Parse(musteriIdString);

            // Repository'i çağır
            int siparisId = await _siparisRepo.SiparisiTamamla(musteriId);

            if (siparisId > 0)
            {
                // Başarılıysa, teşekkür sayfasına yönlendirme linki gönderiyoruz
                return Json(new { success = true, redirectUrl = $"/Siparis/Tesekkurler?siparisId={siparisId}" });
            }
            else
            {
                return Json(new { success = false, message = "Sepetiniz boş veya zaten onaylanmış." });
            }
        }

        // 2. Teşekkür Sayfası (View)
        [HttpGet]
        public IActionResult Tesekkurler(int siparisId)
        {
            ViewBag.SiparisId = siparisId;
            return View();
        }
    }
}
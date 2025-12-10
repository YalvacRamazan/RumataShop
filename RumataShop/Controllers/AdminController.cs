using Microsoft.AspNetCore.Mvc;
using RumataShop.Repositories;
using RumataShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace RumataShop.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IUrunRepository _urunRepo;
        private readonly IKategoriRepository _kategoriRepo; // EKLENDİ

        // Constructor'a (Yapıcı Metot) ekleme yapıyoruz
        public AdminController(IUrunRepository urunRepo, IKategoriRepository kategoriRepo)
        {
            _urunRepo = urunRepo;
            _kategoriRepo = kategoriRepo;
        }

        // --- ANA SAYFA (LİSTELEME) ---
        public async Task<IActionResult> Index()
        {
            // 1. Kategorileri Çek -> ViewBag'e at (Dropdown listesi için)
            // DİKKAT: Bunu View() içine göndermiyoruz, ViewBag'e atıyoruz.
            ViewBag.Kategoriler = await _kategoriRepo.TumKategorileriGetir();

            // 2. Ürünleri Çek -> Model olarak View'a gönder (Tablo için)
            var urunler = await _urunRepo.TumUrunleriGetir();

            // DİKKAT: Buraya "urunler" listesini yazmalısın!
            return View(urunler);
        }

        // --- KATEGORİ EKLEME (POST) ---
        [HttpPost]
        public async Task<IActionResult> KategoriEkle(string KategoriAdi)
        {
            if (!string.IsNullOrEmpty(KategoriAdi))
            {
                await _kategoriRepo.Ekle(KategoriAdi);
                TempData["Mesaj"] = "Kategori başarıyla eklendi!";
            }
            return RedirectToAction("Index");
        }

        // --- KATEGORİ SİLME ---
        public async Task<IActionResult> KategoriSil(int id)
        {
            await _kategoriRepo.Sil(id);
            TempData["Hata"] = "Kategori silindi!";
            return RedirectToAction("Index");
        }

        // Urun ekle

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UrunEkle(Urun yeniUrun, string Fiyat)
        {
            // C# tarafında virgül/nokta karmaşasını manuel çözüyoruz
            if (!string.IsNullOrEmpty(Fiyat))
            {
                // 1. Önce varsa noktaları (binlik ayırıcı) sil
                // 2. Virgülü decimal separator olarak kabul et
                string temizFiyat = Fiyat.Replace(".", ""); // Binlik noktaları at (Örn: 1.200,50 -> 1200,50)

                if (decimal.TryParse(temizFiyat, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("tr-TR"), out decimal sonuc))
                {
                    yeniUrun.Fiyat = sonuc;
                }
            }

            if (yeniUrun.Fiyat > 0 && !string.IsNullOrEmpty(yeniUrun.UrunAdi))
            {
                await _urunRepo.UrunEkle(yeniUrun);
                TempData["Mesaj"] = "Ürün başarıyla eklendi!";
            }
            else
            {
                TempData["Hata"] = "Ürün eklenemedi. Fiyat formatını kontrol edin.";
            }
            return RedirectToAction("Index");
        }
        // Ürün sil

        public async Task<IActionResult> UrunSil(int id)
        {
            await _urunRepo.UrunSil(id); // yeni metod
            TempData["Hata"] = "Ürün silindi.";
            return RedirectToAction("Index");
        }
        // --- ÜRÜN DÜZENLEME SAYFASINI AÇ (GET) ---
        [HttpGet]
        public async Task<IActionResult> UrunDuzenle(int id)
        {
            // 1. Düzenlenecek ürünü veritabanından çek
            var urun = await _urunRepo.UrunDetayGetir(id);
            if (urun == null) return RedirectToAction("Index");

            // 2. Kategorileri de gönder (Select kutusu için)
            ViewBag.Kategoriler = await _kategoriRepo.TumKategorileriGetir();

            // 3. Ürünü sayfaya model olarak gönder
            return View(urun);
        }

        [HttpPost]
        public async Task<IActionResult> UrunGuncelle(Urun guncelUrun, string Fiyat)
        {
            // 1. ID KONTROLÜ (En sık yapılan hata burasıdır)
            if (guncelUrun.UrunId <= 0)
            {
                TempData["Hata"] = "Hata: Ürün ID'si bulunamadı! Güncelleme yapılamadı.";
                return RedirectToAction("Index");
            }

            // 2. Fiyat Formatlama (Nokta/Virgül düzeltmesi)
            if (!string.IsNullOrEmpty(Fiyat))
            {
                string duzeltilmisFiyat = Fiyat.Replace(".", ",");
                if (decimal.TryParse(duzeltilmisFiyat, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("tr-TR"), out decimal sonuc))
                {
                    guncelUrun.Fiyat = sonuc;
                }
            }

            // 3. Güncelleme İşlemi
            try
            {
                await _urunRepo.UrunGuncelle(guncelUrun);
                TempData["Mesaj"] = "Ürün başarıyla güncellendi! ✅";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Bir sorun oluştu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
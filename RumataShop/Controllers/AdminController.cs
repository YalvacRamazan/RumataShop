using Microsoft.AspNetCore.Mvc;
using RumataShop.Repositories;

namespace RumataShop.Controllers
{
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
            // Kategorileri veritabanından çekiyoruz
            var kategoriler = await _kategoriRepo.TumKategorileriGetir();

            // View'a gönderiyoruz
            return View(kategoriler);
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
    }
}
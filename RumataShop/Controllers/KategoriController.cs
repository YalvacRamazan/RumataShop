using Microsoft.AspNetCore.Mvc;
using RumataShop.Repositories;
using RumataShop.Models;

namespace RumataShop.Controllers
{
        public class KategoriController : Controller
        {
            private readonly IKategoriRepository _kategoriRepo;

            public KategoriController(IKategoriRepository kategoriRepo)
            {
                _kategoriRepo = kategoriRepo;
            }

            public async Task<IActionResult> Index()
            {
                var kategoriler = await _kategoriRepo.TumKategorileriGetir();
                return View(kategoriler);
            }
        // 2 Ekleme Sayfasını göster (GET)
        //Kullanıcı "Yeni Ekle butonuna basınca boş form açılır
        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        //3. Ekleme İŞlemini yap (POST)
        // Kullanıcı formu doldurup "kaydet'e basınca burası çalışır
        [HttpPost]
        public async Task<IActionResult> Ekle(Kategori yeniKategori)
        {
            if (ModelState.IsValid)
            {
                await _kategoriRepo.KategoriEkle(yeniKategori);
                return RedirectToAction("Index"); // Kayıttan sonra listeye dön
            }
            return View(yeniKategori);
        }



    }
    
}

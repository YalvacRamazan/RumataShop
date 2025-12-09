using Microsoft.AspNetCore.Mvc;
using RumataShop.Repositories;

namespace RumataShop.Controllers
{
    public class UrunController : Controller
    {
        // Repository kullanmak
        private readonly IUrunRepository _urunRepo;

        // Constructor (Yapıcı Metot):
        // Proje çalıştığında bize otomatik olarak UrunRepository' yi getirir
        public UrunController(IUrunRepository urunRepo)
        {
            _urunRepo = urunRepo;
        }

        // Sayfa açıldığında çalışacak kısım
        public async Task<IActionResult> Index()
        {
            // 1. Veritabanından ürünleri çek
            var urunler = await _urunRepo.TumUrunleriGetir();

            // 2. Gelen listeyi View' a (ekrana) gönder
            return View(urunler);

            
        }
        //URL' den bir ID bekliyoruz 
        [HttpGet]
        public async Task<IActionResult> Detay(int id)
        {
            // 1. Ürünü veritabanından çek
            var urun = await _urunRepo.UrunDetayGetir(id);

            // 2. Eğer ürün yoksa (veya silinmişse) ana sayfaya at
            if (urun == null) return RedirectToAction("Index");

            // 3. Ürünü View sayfasına gönder
            return View(urun);
        }
    }
}

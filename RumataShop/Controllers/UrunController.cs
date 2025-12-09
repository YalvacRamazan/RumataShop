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
        public async Task<IActionResult>Detay(int id)
        {
            //Repository'deki o yazdığımız metodu kullanıyoruz
            var urun = await _urunRepo.UrunDetayGetir(id);

            // Eğer ürün bulunmazsa (linke resgele sayi yazarlarsa) Ana sayfaya at
            if (urun == null)
            {
                return RedirectToAction("Index");
            }
            return View(urun);
        }
    }
}

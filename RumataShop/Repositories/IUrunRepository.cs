using RumataShop.Models;

namespace RumataShop.Repositories
{
    public interface IUrunRepository
    {
        //Bu bir sözleşmedir. Diyoruz ki: Bu interface'i kullanan herkes
        // tüm ürünleri getiren bir metoda sahip olmak zorundadır
        Task<IEnumerable<Urun>> TumUrunleriGetir();

        Task<IEnumerable<Urun>> KategoriyeGoreGetir(int kategoriId);

        Task<IEnumerable<Urun>> UrunAra(string kelime);

        Task<Urun> UrunDetayGetir(int Id);

        Task UrunEkle(Urun yeniUrun);

        Task UrunSil(int id);
        Task UrunGuncelle(Urun guncelUrun);
    }
}

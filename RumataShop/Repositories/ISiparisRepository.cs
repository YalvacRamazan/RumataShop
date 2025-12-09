using RumataShop.Models;

namespace RumataShop.Repositories
{
    public interface ISiparisRepository 
    {
        // Muşterinin kendi siparislerini görmesi için
        Task<IEnumerable<Siparis>> MusteriSiparisleriniGetir(int musteriId);

        // Yeni Siparis Olusturma (Geriye olusan siparisin ID'sini döner)
        
        Task SepeteEkle(int musteriId, int urunId, int adet);

        // Siparişin içine ürün (detay) ekleme
        Task SiparisDetayEkle(SiparisDetay detay);

        // Siparisin icindeki urunleri gorme (Fatura detayı gibi)
        Task<IEnumerable<SiparisDetay>> SiparisDetaylariniGetir(int siparisId);

        Task<IEnumerable<SepetDetayDto>> SepetiGetir(int musteriId);
        Task SepetGuncelle(int musteriId, int urunId, int adetDegisimi);

        Task<int> SiparisiTamamla(int musteriId);
    }
}

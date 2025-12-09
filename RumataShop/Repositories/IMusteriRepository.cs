using RumataShop.Models;

namespace RumataShop.Repositories
{
    public interface IMusteriRepository
    {
        // Kayır Ol
        Task MusteriKayit(Musteri musteri);

        //Giriş Yap (Geriye giriş yapan müşterinin bilgisini döner )
        Task<Musteri> MusteriLogin(string email, string sifre);

        // Şifremi Unuttum(Token oluşturur)
        Task TokenOlustur(int musteriId, string token);

        // Şifre Yenileme (Yeni şifreyi kaydeder)
        Task SifreYenile(string token, string yeniSifre);
    }
}

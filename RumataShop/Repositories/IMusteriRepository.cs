using RumataShop.Models;

namespace RumataShop.Repositories
{
    public interface IMusteriRepository
    {
        Task Ekle(Musteri musteri);
        Task<Musteri> EmailIleGetir(string email);
        Task TokenOlustur(int musteriId, string token);
        Task SifreYenile(string token, string yeniSifre);

        // Eski
        Task<Musteri> MusteriLogin(string email, string sifre);
    }
}
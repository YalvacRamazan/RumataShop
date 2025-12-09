using RumataShop.Models;
namespace RumataShop.Repositories
{
    public interface IKategoriRepository
    {
        

    
        public interface IKategoriRepository
        {
            Task<IEnumerable<Kategori>> TumKategorileriGetir();
            Task Ekle(string ad);
            Task Sil(int id);
        }
    


}
}

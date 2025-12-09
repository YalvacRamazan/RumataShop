using Dapper;
using RumataShop.Context;
using RumataShop.Models;
using System.Data;

namespace RumataShop.Repositories
{
    public class UrunRepository : IUrunRepository
    {
        private readonly DapperContext _context;

        // Constructor: DapperContext'i buraya çağırıyoruz 
        public UrunRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Urun>> TumUrunleriGetir()
        {
            // DB deki procedure
            var query = "sp_Tum_Urunleri_getir";

            using (var connection = _context.CreateConnection())
            {
                // Dapper kullanılıyor
                var urunler = await connection.QueryAsync<Urun>(query, commandType: CommandType.StoredProcedure);

                return urunler.ToList();
            }
        }
        // Kategoriye göre listeleme
        public async Task<IEnumerable<Urun>> KategoriyeGoreGetir(int kategoriId)
        {
            var query = "sp_Urunleri_Getir_ByKategoriId";

            // SQL' de parametre adı @KatId ise burası doğru
            var parameters = new { KatId = kategoriId };

            using (var connection = _context.CreateConnection())
            {
                var urunler = await connection.QueryAsync<Urun>(query, parameters, commandType: CommandType.StoredProcedure);
                return urunler.ToList();
            }
        }

        // Ürün Ara

        public async Task<IEnumerable<Urun>> UrunAra(string kelime)
        {
            var query = "sp_Urun_Ara";

            // SQL ' de parametre adı @Kelime veya @Ad ise burayı düzelt
            var parameters = new { Kelime = kelime };

            using (var connection = _context.CreateConnection())
            {
                var urunler = await connection.QueryAsync<Urun>(query, parameters, commandType: CommandType.StoredProcedure);
                return urunler.ToList();
            }
        }

        // Detay GETİR

        public async Task<Urun> UrunDetayGetir(int id)
        {
            var query = "sp_Urun_Detay_Getir";

            // SQL' de parametre adı @Id mi @UrunId mi?
            var parameters = new { UrunId = id };

            using (var connection = _context.CreateConnection())
            {
                var urun = await connection.QuerySingleOrDefaultAsync<Urun>(query, parameters, commandType: CommandType.StoredProcedure);

                return urun;
            }
        }

        // Urun Ekle

        public async Task UrunEkle(Urun yeniUrun)
        {
            var query = "sp_Urun_Ekle";
            var parameters = new DynamicParameters();

            //Burası çok önemli : SQL Prosedürlerindeki parametre adlarını (tırnak içlerini) kontrol et)
            // Örnek sql de @UrunAdi yazıyorsa burası "UrunAdi" olmali
            parameters.Add("UrunAdi", yeniUrun.UrunAdi);
            parameters.Add("KategoriIdR", yeniUrun.KategorildR);
            parameters.Add("Fiyat", yeniUrun.Fiyat);
            parameters.Add("Aciklama", yeniUrun.Aciklama);
            parameters.Add("StokAdedi", yeniUrun.StokAdedi);
            parameters.Add("ResimUrl", yeniUrun.ResimUrl);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

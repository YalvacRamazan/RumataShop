using Dapper;
using Microsoft.EntityFrameworkCore;
using RumataShop.Context;
using RumataShop.Models;
using System.Data;


namespace RumataShop.Repositories
{
    public class SiparisRepository : ISiparisRepository
    {
        private readonly DapperContext _context;
        public SiparisRepository(DapperContext context)
        {
            _context = context;
        }

        // Müşterinin siparisleri
        public async Task<IEnumerable<Siparis>> MusteriSiparisleriniGetir(int musteriId)
        {
            var query = "sp_Musteri_Siparisleri_Getir";
            var parameters = new { MusteriId = musteriId };

            using (var connection = _context.CreateConnection())
            {
                var siparisler = await connection.QueryAsync<Siparis>(query, parameters, commandType: CommandType.StoredProcedure);
                return siparisler.ToList();
            }
        }

        
        
        // Bu metodu class'ın içine ekle
        public async Task SepeteEkle(int musteriId, int urunId, int adet)
        {
            // SQL'deki prosedür adımız:
            var query = "sp_Siparis_Olustur";

            var parameters = new DynamicParameters();
            parameters.Add("MusteriId", musteriId);
            parameters.Add("UrunId", urunId);
            parameters.Add("Adet", adet);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // Siparis Detay Ekle (Sepetteki ürünleriDB ' ye yazar)
        public async Task SiparisDetayEkle(SiparisDetay detay)
        {
            var query = "sp_Siparis_Detay_Ekle";
            var parameters = new DynamicParameters();

            parameters.Add("SiparisId", detay.SiparisIdR);
            parameters.Add("UrunId", detay.UrunIdR);
            parameters.Add("Adet", detay.Adet);
            parameters.Add("BirimFiyat", detay.BirimFiyat);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // Siparis Detaylarini getir
        public async Task<IEnumerable<SiparisDetay>> SiparisDetaylariniGetir(int siparisId)
        {
            var query = "sp_Siparis_Detaylarini_Getir";
            var parameters = new { SiparisId = siparisId };

            using (var connection = _context.CreateConnection())
            {
                var detaylar = await connection.QueryAsync<SiparisDetay>(query, parameters, commandType: CommandType.StoredProcedure);
                return detaylar.ToList();
            }
        }

        public async Task<IEnumerable<SepetDetayDto>> SepetiGetir(int musteriId)
        {
            var query = "sp_Sepet_Getir";
            var parameters = new DynamicParameters();
            parameters.Add("MusteriId", musteriId);

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<SepetDetayDto>(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task SepetGuncelle(int musteriId, int urunId, int adetDegisimi)
        {
            var query = "sp_Sepet_Guncelle";
            var parameters = new DynamicParameters();
            parameters.Add("MusteriId", musteriId);
            parameters.Add("UrunId", urunId);
            parameters.Add("AdetDegisimi", adetDegisimi);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<int> SiparisiTamamla(int musteriId)
        {
            var query = "sp_Siparis_Tamamla";
            var parameters = new { MusteriId = musteriId };

            using (var connection = _context.CreateConnection())
            {
                // QuerySingleOrDefault kullanıyoruz çünkü geriye ID dönüyor
                var result = await connection.QuerySingleOrDefaultAsync<int>(query, parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

      

        public async Task<List<SiparisDetayProcedureModel>> SiparisDetayGetirSP(int siparisId)
        {
        // 1. Adım: DapperContext'ten bir bağlantı oluşturuyoruz
        using (var connection = _context.CreateConnection())
        {
            // 2. Adım: Parametreleri hazırlıyoruz
            var parameters = new DynamicParameters();
            parameters.Add("SiparisId", siparisId);

            // 3. Adım: Dapper'ın Query metoduyla prosedürü çağırıyoruz
            // "CommandType.StoredProcedure" diyerek bunun bir SP olduğunu belirtiyoruz
            var sonuc = await connection.QueryAsync<SiparisDetayProcedureModel>(
                "sp_Siparis_Tam_Detay_Getir",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return sonuc.ToList();
        }
    }



}
}

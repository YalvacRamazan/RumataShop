using Dapper;
using RumataShop.Context;
using RumataShop.Models;
using System.Data;

namespace RumataShop.Repositories
{
    public class MusteriRepository : IMusteriRepository
    {
        private readonly DapperContext _context;

        public MusteriRepository(DapperContext context)
        {
            _context = context;
        }

        // --- KAYIT İŞLEMİ ---
        public async Task Ekle(Musteri musteri)
        {
            var query = "sp_Musteri_Kayit";
            var parameters = new DynamicParameters();

            // Model isimleri ile birebir aynı:
            parameters.Add("MusteriAd", musteri.MusteriAd);
            parameters.Add("MusteriSoyad", musteri.MusteriSoyad);
            parameters.Add("MusteriMail", musteri.MusteriMail);

            // Şifre hashlenmiş olarak geliyor
            parameters.Add("MusteriSifreHash", musteri.MusteriSifreHash);

            parameters.Add("MusteriKayitTarihi", musteri.MusteriKayitTarihi);

            // Eğer veritabanında "Rol" ve "AktifMi" parametreleri de varsa ekle:
            // parameters.Add("Rol", musteri.Rol ?? "Musteri"); 
            // parameters.Add("AktifMi", musteri.AktifMi); 

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // --- EMAIL İLE GETİR (Giriş kontrolü için) ---
        public async Task<Musteri> EmailIleGetir(string email)
        {
            // SQL'de parametre adı @Email ise:
            var query = "sp_Musteri_Getir_ByEmail";
            var parameters = new { Email = email };

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Musteri>(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // --- DİĞER METOTLARIN (Aynen kalabilir) ---
        public async Task TokenOlustur(int musteriId, string token)
        {
            var query = "sp_Sifre_Token_Olustur";
            var parameters = new { MusteriId = musteriId, Token = token };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task SifreYenile(string token, string yeniSifre)
        {
            var query = "sp_Sifre_Yenile";
            var parameters = new { Token = token, YeniSifreHash = yeniSifre };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // Login metodu artık gereksiz ama Interface hatası vermemesi için durabilir
        public async Task<Musteri> MusteriLogin(string email, string sifre)
        {
            return null; // Kullanmıyoruz
        }
    }
}
using Dapper;
using NuGet.Common;
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

        // Kayıt Ol
        public async Task MusteriKayit(Musteri musteri)
        {
            var query = "sp_Musteri_Kayit";
            var parameters = new DynamicParameters();

            //SQL Parametre ADları vs Model Adları
            parameters.Add("MusteriAd", musteri.MusteriAd);
            parameters.Add("MusteriSoyad", musteri.MusteriSoyad);
            parameters.Add("MusteriMail", musteri.MusteriMail);
            parameters.Add("MusteriSifreHash", musteri.MusteriSifreHash);
            parameters.Add("MusteriKayitTarihi", musteri.MusteriKayitTarihi);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // Giriş Yap (Login)
        public async Task<Musteri> MusteriLogin(string email, string sifre)
        {
            var query = "sp_Musteri_Login";

            //SQL ' deki parametre adlarını kontrol et.
            var parameters = new { Mail = email, Sifre = sifre };

            using (var connection = _context.CreateConnection())
            {
                //Eğer kullanıcı bulunursa bilgilerini döner, bulunmazsa null döner
                var musteri = await connection.QuerySingleOrDefaultAsync<Musteri>(query, parameters, commandType: CommandType.StoredProcedure);
                return musteri;
            }
        }

        public async Task TokenOlustur(int musteriId, string token)
        {
            var query = "sp_Sifre_Token_Olustur";
            var parameters = new { MusteriId = musteriId, Token = token };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // 4 Sİfre Yenile yeni sifreyi kaydet
        public async Task SifreYenile(string token, string yeniSifre)
        {
            var query = "sp_Sifre_Yenile";
            //SQL' de Token'e göre bulup şifreyi mi güncelliyor parametre adlarına bak
            var parameters = new { Token = token, YeniSifreHash = yeniSifre };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

            }
        }

    }
}

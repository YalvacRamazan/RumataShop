using Dapper;
using RumataShop.Context;
using RumataShop.Models;
using System.Data;

namespace RumataShop.Repositories
{
    public class KategoriRepository : IKategoriRepository
    {
        private readonly DapperContext _context;

        public KategoriRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Kategori>> TumKategorileriGetir()
        {
            var query = "sp_Kategorileri_Getir";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Kategori>(query, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Ekle(string ad)
        {
            var query = "sp_Kategori_Ekle";

            // DİKKAT: Senin prosedürün @KategoriAdi bekliyor!
            // Burayı senin SQL'ine göre düzelttim:
            var parameters = new { KategoriAdi = ad };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Sil(int id)
        {
            var query = "sp_Kategori_Sil";
            var parameters = new { Id = id }; // Silme prosedüründeki parametre @Id
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
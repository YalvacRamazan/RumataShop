namespace RumataShop.Models
{
    public class SifreYenileme
    {
        public int SifreId { get; set; }
        public int MusteriIdR { get; set; }
        public string Token { get; set; }
        public DateTime OlusurulmaTarihi { get; set; }
        public DateTime GecerlilikTarihi { get; set; }
        public bool KullanildiMi { get; set; }
    }
}

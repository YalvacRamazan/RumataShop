namespace RumataShop.Models
{
    public class Musteri
    {
        public int MusteriId { get; set; }
        public string MusteriAd { get; set; }
        public string MusteriSoyad { get; set; }
        public string MusteriMail { get; set; }
        public DateTime MusteriKayitTarihi { get; set; }
        public string MusteriSifreHash { get; set; }
        public string Rol { get; set; }
        public bool AktifMi { get; set; }

     }
}

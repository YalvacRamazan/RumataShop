namespace RumataShop.Models
{
    public class SepetDetayDto
    {
        public int SiparisDetayId { get; set; }
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public string ResimUrl { get; set; }
        public int Adet { get; set; }
        public decimal Fiyat { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}
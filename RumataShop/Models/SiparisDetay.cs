namespace RumataShop.Models
{
    public class SiparisDetay
    {
        public int SiparisDetayId { get; set; }
        public int SiparisIdR { get; set; }
        public int UrunIdR { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public string UrunAdi { get; set; }

        public decimal SatirToplami { get; set; }
    }
    
}

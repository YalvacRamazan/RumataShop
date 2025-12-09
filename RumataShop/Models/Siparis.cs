namespace RumataShop.Models
{
    public class Siparis
    {
        public int SiparisId { get; set; }
        public int MusteriIdRF { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }

        public string Durum { get; set; }
    }
}

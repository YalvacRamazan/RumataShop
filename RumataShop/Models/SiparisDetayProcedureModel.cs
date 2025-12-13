namespace RumataShop.Models
{
    public class SiparisDetayProcedureModel
    {
        public int SiparisId { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string SiparisDurum { get; set; }
        public decimal ToplamTutar { get; set; }
        public string UrunAd { get; set; }
        public string ResimUrl { get; set; }
        public decimal SatisFiyati { get; set; }
        public int Adet { get; set; }
        public decimal AraToplam { get; set; }

    }
}

namespace RumataShop.Models
{
    public class Urun
    {
        public int UrunId { get; set; }
        public string UrunAdi{ get; set; }
        public int KategorildR { get; set; }
        public decimal Fiyat { get; set; }
        public int StokAdedi { get; set; }
        public string ResimUrl { get; set; }
        public string Aciklama { get; set; }

    }
}

namespace DPT.MVC.Models
{
    public class PAYMENTRECEIPTCHARGES
    {
        public int ID { get; set; }
        public int? PAYMENTHDRID { get; set; }
        public int? PAYMENTRECEIPTID { get; set; }
        public int? REQUESTID { get; set; }
        public int? CHARGETYPEID { get; set; }
        public double? AMOUNT { get; set; }
        public string? REFERENCENO { get; set; }
        public DateTime? REFERENCEDATE { get; set; }
        public string? REMARKS1 { get; set; }
        public string? REMARKS2 { get; set; }
        public DateTime? CREATED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? LASTUPDATED { get; set; }
        public int? LASTUPDATEDBY { get; set; }
    }
}

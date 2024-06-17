namespace DPT.MVC.Models
{
    public class PAYMENTRECEIPTS
    {
        public int ID { get; set; }
        public int HDRID { get; set; }
        public int? REQUESTID { get; set; }
        public int? SEQUENCEID { get; set; }
        public string? RECEIPTNO { get; set; }
        public DateTime? RECEIPTDATE { get; set; }
        public double? RECEIPTAMOUNT { get; set; }
        public string? REMARKS1 { get; set; }
        public string? REMARKS2 { get; set; }
        public DateTime? CREATED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? LASTUPDATED { get; set; }
        public int? LASTUPDATEDBY { get; set; }
    }
}

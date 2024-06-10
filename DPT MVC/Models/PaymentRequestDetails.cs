namespace DPT.MVC.Models
{
    public class PaymentRequestDetails
    {
        public int CHARGETYPEID1 { get; set; }
        public double? AMOUNT1 { get; set; }
        public int CHARGETYPEID2 { get; set; }
        public double? AMOUNT2 { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string? REMARKS1 { get; set; }
        public string? REMARKS2 { get; set; }
        public DateTime? CREATED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? LASTUPDATED { get; set; }
        public int? LASTUPDATEDBY { get; set; }
        public string? JOBNO { get; set; }
        public string? HAWBNO { get; set; }
        public int? CUSTOMERID { get; set; }
        public string? CUSTOMERNAME { get; set; }
        public string? BAYANNO { get; set; }
        public double? TOTALREQUESTAMOUNT { get; set; }
        public string? REQUESTEDBY { get; set; }
        public double? PAYMENTAMOUNT { get; set; }
    }
}

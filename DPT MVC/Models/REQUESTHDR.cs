namespace DPT.MVC.Models
{
    public class REQUESTHDR
    {
        public int ID { get; set; }
        public string? REQUESTNO { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string? JOBNO { get; set; }
        public string? HAWBNO { get; set; }
        public int? CUSTOMERID { get; set; }
        public string? CUSTOMERNAME { get; set; }
        public string? BAYANNO { get; set; }
        public double? TOTALREQUESTAMOUNT { get; set; }
        public int? PAYMENTTYPEID { get; set; }
        public string? REQUESTEDBY { get; set; }
        public string? REMARKS1 { get; set; }
        public string? REMARKS2 { get; set; }
        public double? PAYMENTAMOUNT { get; set; }
        public bool? EXCEPTION { get; set; }
        public string? EXCEPTIONCOMMENTS { get; set; }
        public string? EXCEPTIONAPPROVEDBY { get; set; }
        public DateTime? EXCEPTIONAPPROVEDON { get; set; }
        public string? STATUS { get; set; }
        public DateTime? CREATED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? LASTUPDATED { get; set; }
        public int? LASTUPDATEDBY { get; set; }
    }
}

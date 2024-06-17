namespace DPT.MVC.Models
{
    public class PAYMENTHDR
    {
        public int ID { get; set; }
        public string? TRANNO { get; set; }
        public DateTime? TRANDATE { get; set; }
        public int REQUESTID { get; set; }
        public string? REMARKS1 { get; set; }
        public string? REMARKS2 { get; set; }
        public string? PAIDBY { get; set; }
        public DateTime? CREATED { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? LASTUPDATED { get; set; }
        public int? LASTUPDATEDBY { get; set; }
    }
}

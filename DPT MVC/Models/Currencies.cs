namespace DPT.MVC.Models
{
    public class Currencies
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string? Name { get; set; }
        public string? DecimalsInWords { get; set; }
        public string? Symbol { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}

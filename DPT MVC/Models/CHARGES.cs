namespace DPT.MVC.Models
{
    public class CHARGES
    {
        public int Id { get; set; }
        public string? TYPE { get; set; }
        public string? Name { get; set; }
        public string? REMARKS { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}

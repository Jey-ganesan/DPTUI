
namespace DPT.MVC.Models
{
    public class Country: CommonFieldsInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }

        public int? CurrencyId { get; set; }
        public bool? IsActive { get; set; }
    }
}

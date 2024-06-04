using System.ComponentModel.DataAnnotations.Schema;

namespace DPT.MVC.Models
{
    public class Customer:CommonFieldsInfo
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        [NotMapped]
        public string? DisplayName
        {
            get
            {
                return ShortName + " - [" + Name + "]";
            }
        }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        [NotMapped]
        public string? CountryName { get; set; }
        public string? PrimaryContactName { get; set; }
        public string? PrimaryContactNo { get; set; }
        public string? PrimaryContactMailId { get; set; }
        public string? SecondaryContactName { get; set; }
        public string? SecondaryContactNo { get; set; }
        public string? SecondaryContactMailId { get; set; }
        public int? CreditDays { get; set; }
        public double? CreditLimit { get; set; }
        public int? ParentCustomerId { get; set; }
        [NotMapped]
        public string? ParentCustomerName { get; set; }
        public string? Code1 { get; set; }
        public string? Code2 { get; set; }
        public string? Code3 { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsGeneric { get; set; }
        public string? Cpc { get; set; }
    }
}


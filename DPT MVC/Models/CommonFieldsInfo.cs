using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPT.MVC.Models
{
    public class CommonFieldsInfo
    {
        public DateTime? Created { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? LastUpdatedBy { get; set; }

        [NotMapped]
        public string? CreatedByName { get; set; }
        [NotMapped]
        public string? LastUpdatedByName { get; set; }
    }
}

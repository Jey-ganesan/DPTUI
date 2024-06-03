using System.ComponentModel.DataAnnotations.Schema;

namespace DPT.MVC.Models
{
    public class MenuInfo
    {
        public int slno { get; set; }
        public string? menuType { get; set; }
        public string? menuName { get; set; }
        public int? parentslno { get; set; }
        [NotMapped]
        public string? parentName { get; set; }
        public int? orderBy { get; set; }
        public bool isActive { get; set; }
        public string? pageName { get; set; }
    }
}

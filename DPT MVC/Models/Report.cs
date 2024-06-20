using System.Data;

namespace DPT.MVC.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SPName { get; set; }
        public bool DropDownToDisplay { get; set; }
        public DataTable Result { get; set; }
    }
}

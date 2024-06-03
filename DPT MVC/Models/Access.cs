namespace DPT.MVC.Models
{
    public class Access
    {
        public bool view { get; set; } = true;
        public bool add { get; set; } = true;
        public bool edit { get; set; } = true;
        public bool delete { get; set; } = true;
    }
}

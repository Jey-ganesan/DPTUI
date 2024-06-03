namespace DPT.MVC.Models
{
 
        public class Permission
        {
            public int id { get; set; }
            public int userId { get; set; }
            public int menuId { get; set; }
            public bool view { get; set; } = false;
            public bool add { get; set; } = false;
            public bool edit { get; set; } = false;
            public bool delete { get; set; } = false;
            public int parentId { get; set; }
            public int sortOrder { get; set; }
            public string menuName { get; set; }
            public string pageName { get; set; }
            public DateTime? created { get; set; }
            public int? createdBy { get; set; }
            public DateTime? lastUpdated { get; set; }
            public int? lastUpdatedBy { get; set; }
        }

       

        public class PermissionPostDTO
        {
            public int id { get; set; }
            public int userId { get; set; }
            public int menuId { get; set; }
            public bool view { get; set; } = false;
            public bool add { get; set; } = false;
            public bool edit { get; set; } = false;
            public bool delete { get; set; } = false;
            public int? parentId { get; set; } = 1;
            public int? sortOrder { get; set; } = 0;
            public DateTime? created { get; set; }
            public int? createdBy { get; set; }
            public DateTime? lastUpdated { get; set; }
            public int? lastUpdatedBy { get; set; }
        }
    
}

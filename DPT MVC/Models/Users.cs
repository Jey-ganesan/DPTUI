﻿namespace DPT.MVC.Models
{
    public class Users
    {
        public int id { get; set; }
        public string? displayName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? userRole { get; set; }
        public bool? iaActive { get; set; }
        public string? transPrefix { get; set; }
        public string? dashboardType { get; set; }
    }
}

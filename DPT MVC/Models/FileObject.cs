namespace DPT.MVC.Models
{
    public class FileObject
    {
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public string? Base64Content { get; set; }
    }
    public class FilesInfo
    {
        public int? id { get; set; }
        public string transName { get; set; }
        public int? transId { get; set; }
        public string fileName { get; set; }
        public string folderName { get; set; }
        public double? sizeInKB { get; set; }
        public int attachmentTypeSlNo { get; set; }
        public object file { get; set; }
    }
}

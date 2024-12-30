namespace DirectoryChangeMonitoring.Models
{
    public class FileDetail
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModified { get; set; }
        public int Version { get; set; }
    }
}

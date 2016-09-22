using System;

namespace webcore.Models
{
    public class VideoViewModel
    {
        public string Href { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.Now;

    }
}
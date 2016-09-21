using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Models
{
    public class Video
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Href { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }

        public Video()
        {

        }
    }
}

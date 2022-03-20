using System.ComponentModel.DataAnnotations.Schema;

namespace ImageResizeUpload.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Dp { get; set; }
    }
}

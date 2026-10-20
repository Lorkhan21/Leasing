using System.ComponentModel.DataAnnotations;

namespace Leasing.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        public int PropertyId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation
        public Property? Property { get; set; }
    }
}
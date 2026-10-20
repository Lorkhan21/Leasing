using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leasing.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        public string Price { get; set; } = string.Empty;

        public string PriceSuffix { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        // NEW: Address extras
        public string? StateCounty { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; } = "Philippines";

        public string PropertyType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }

        // NEW: Overview fields
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public string? AreaSize { get; set; }

        // NEW: Rich content
        public string? Description { get; set; }
        public string? WarehouseHighlights { get; set; }
        public string? RateDetails { get; set; }
        public string? Terms { get; set; }
        public string? LeaseTerm { get; set; }

        public string? AgentName { get; set; }
        public string? AgentEmail { get; set; }
        public string? AgentImage { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.Now;

        // NEW: Track when the property was last edited
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public List<PropertyFeature> Features { get; set; } = new();
        public List<PropertyImage> Images { get; set; } = new();
    }

    public class PropertyFeature
    {
        [Key]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public Property? Property { get; set; }
    }

    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        [NotMapped]
        public int PropertyCount { get; set; }
    }
}
using Leasing.Models;

namespace Leasing.Models
{
    public class HomeViewModel
    {
        public List<Property> FeaturedProperties { get; set; } = new();
        public List<City> Cities { get; set; } = new();
    }
}
using Microsoft.EntityFrameworkCore;
using Leasing.Data;
using Leasing.Models;

namespace Leasing.Services
{
    public interface IPropertyService
    {
        List<Property> GetFeaturedProperties();
        List<Property> GetPropertiesByCity(string city);
        List<City> GetAllCities();
        Property? GetPropertyById(int id);
    }

    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;

        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Property> GetFeaturedProperties()
        {
            return _context.Properties
                .Include(p => p.Features)
                .Where(p => p.IsFeatured)
                .ToList();
        }

        public List<Property> GetPropertiesByCity(string city)
        {
            return _context.Properties
                .Include(p => p.Features)
                .Where(p => p.City.ToLower() == city.ToLower())
                .ToList();
        }

        public List<City> GetAllCities()
        {
            return _context.Properties
                .GroupBy(p => p.City)
                .Select(g => new City
                {
                    Name = g.Key,
                    PropertyCount = g.Count(),  // This is a temporary property
                    Description = g.First().PropertyType == "Warehouse" ? "Industrial & warehouses" :
                                  g.First().PropertyType == "Commercial" ? "Commercial & retail" :
                                  "Residential & commercial",
                    ImageUrl = "/image/cities/" + g.Key.ToLower().Replace(" ", "-") + ".jpg"
                })
                .ToList();
        }

        public Property? GetPropertyById(int id)
        {
            return _context.Properties
                .Include(p => p.Features)
                .FirstOrDefault(p => p.Id == id);
        }
    }
}
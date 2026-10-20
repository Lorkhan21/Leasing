using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Leasing.Data;

namespace Leasing.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Listings?type=Warehouse&city=Cavite&status=For+Rent
        public IActionResult Index(string? type, string? city, string? status)
        {
            var query = _context.Properties
                .Include(p => p.Features)
                .AsQueryable();

            // Filter by property type
            if (!string.IsNullOrEmpty(type))
            {
                // Handle "Residential" as a group
                if (type.Equals("Residential", StringComparison.OrdinalIgnoreCase))
                {
                    var residentialTypes = new[] { "Apartment", "Condo", "House" };
                    query = query.Where(p => residentialTypes.Contains(p.PropertyType));
                }
                else
                {
                    query = query.Where(p => p.PropertyType.ToLower() == type.ToLower());
                }
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.ToLower() == city.ToLower());
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status.ToLower() == status.ToLower());
            }

            var properties = query
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.PostedDate)
                .ToList();

            ViewBag.Type = type;
            ViewBag.City = city;
            ViewBag.Status = status;
            ViewBag.Count = properties.Count;

            var titleParts = new List<string>();
            if (!string.IsNullOrEmpty(type)) titleParts.Add(type);
            if (!string.IsNullOrEmpty(city)) titleParts.Add("in " + city);
            ViewBag.PageTitle = titleParts.Any()
                ? string.Join(" ", titleParts)
                : "All Properties";

            return View(properties);
        }
    }
}
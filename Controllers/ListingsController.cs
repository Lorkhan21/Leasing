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
        public IActionResult Index(string? type, string? city, string? status, string? keyword)
        {
            var query = _context.Properties
                .Include(p => p.Features)
                .AsQueryable();

            // 1. Handle the search keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(keyword) ||
                    p.Address.ToLower().Contains(keyword) ||
                    p.City.ToLower().Contains(keyword) ||
                    p.PropertyType.ToLower().Contains(keyword)
                );
            }

            // 2. Handle property type
            if (!string.IsNullOrEmpty(type))
            {
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

            // 3. Handle city filter
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.ToLower() == city.ToLower());
            }

            // 4. Handle status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status.ToLower() == status.ToLower());
            }

            var properties = query
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.PostedDate)
                .ToList();

            // Pass everything to the view
            ViewBag.Type = type;
            ViewBag.City = city;
            ViewBag.Status = status;
            ViewBag.Keyword = keyword; // Pass keyword so the search bar in the listing page can retain the text
            ViewBag.Count = properties.Count;

            var titleParts = new List<string>();
            if (!string.IsNullOrEmpty(keyword)) titleParts.Add($"\"{keyword}\"");
            if (!string.IsNullOrEmpty(type)) titleParts.Add(type);
            if (!string.IsNullOrEmpty(city)) titleParts.Add("in " + city);

            ViewBag.PageTitle = titleParts.Any()
                ? string.Join(" ", titleParts)
                : "All Properties";

            return View(properties);
        }
    }
}
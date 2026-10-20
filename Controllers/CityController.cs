// Controllers/CityController.cs
using Microsoft.AspNetCore.Mvc;
using Leasing.Services;

namespace Leasing.Controllers
{
    public class CityController : Controller
    {
        private readonly IPropertyService _propertyService;

        public CityController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public IActionResult Details(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Home");
            }

            var properties = _propertyService.GetPropertiesByCity(name);

            if (!properties.Any())
            {
                return NotFound();
            }

            ViewBag.CityName = name;
            ViewBag.PropertyCount = properties.Count;
            return View(properties);
        }
    }
}
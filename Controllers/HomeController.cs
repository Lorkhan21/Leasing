// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Leasing.Services;
using Leasing.Models;

namespace Leasing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;

        public HomeController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedProperties = _propertyService.GetFeaturedProperties(),
                Cities = _propertyService.GetAllCities()
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
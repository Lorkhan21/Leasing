using Leasing.Models;
using Leasing.Services;
using Leasing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leasing.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public PropertyController(
            IPropertyService propertyService,
            IEmailService emailService,
            ApplicationDbContext context)
        {
            _propertyService = propertyService;
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var property = _context.Properties
                .Include(p => p.Features)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (property == null) return NotFound();

            var model = new EmailViewModel
            {
                PropertyId = property.Id,
                PropertyTitle = property.Title,
                AgentName = property.AgentName,
                AgentEmail = property.AgentEmail
            };

            ViewBag.Property = property;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendInquiry(EmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var property = _propertyService.GetPropertyById(model.PropertyId);
                ViewBag.Property = property;
                return View("Details", model);
            }

            var result = await _emailService.SendInquiryEmailAsync(model);

            if (result)
                TempData["SuccessMessage"] = "Your inquiry has been sent successfully!";
            else
                TempData["ErrorMessage"] = "Failed to send your inquiry.";

            return RedirectToAction("Details", new { id = model.PropertyId });
        }
    }
}
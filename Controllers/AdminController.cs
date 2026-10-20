using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Leasing.Data;
using Leasing.Models;

namespace Leasing.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Admin
        // List all properties
        public IActionResult Index()
        {
            var properties = _context.Properties
                .Include(p => p.Features)
                .OrderByDescending(p => p.PostedDate)
                .ToList();
            return View(properties);
        }

        // GET: /Admin/Create
        // Show the add property form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Create
        // Save new property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    property.ImageUrl = await SaveImage(imageFile);
                }

                property.PostedDate = DateTime.Now;

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Property '{property.Title}' added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(property);
        }

        // GET: /Admin/Edit/5
        // Show the edit form with existing data
        public IActionResult Edit(int id)
        {
            var property = _context.Properties
                .Include(p => p.Features)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (property == null) return NotFound();

            return View(property);
        }

        // POST: /Admin/Edit/5
        // Save changes to existing property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, IFormFile? imageFile)
        {
            if (id != property.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Handle new image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    property.ImageUrl = await SaveImage(imageFile);
                }
                else
                {
                    // Keep existing image
                    var existing = await _context.Properties
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);
                    property.ImageUrl = existing?.ImageUrl;
                }

                property.UpdatedDate = DateTime.Now;  // Update Time

                _context.Update(property);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Property '{property.Title}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(property);
        }

        // POST: /Admin/Delete/5
        // Delete a property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);

            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Property '{property.Title}' deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper: Save uploaded image to wwwroot/image
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "image");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/image/" + uniqueFileName;
        }


        // Image Gallery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int propertyId, List<IFormFile> imageFiles)
        {
            //  DEBUG LOGGING — will show in Visual Studio Output → Debug
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("UploadImages ACTION HIT");
            System.Diagnostics.Debug.WriteLine($"propertyId = {propertyId}");
            System.Diagnostics.Debug.WriteLine($"imageFiles == null: {imageFiles == null}");
            System.Diagnostics.Debug.WriteLine($"imageFiles count: {imageFiles?.Count ?? 0}");

            if (imageFiles == null || imageFiles.Count == 0)
            {
                TempData["Error"] = "No files were selected.";
                return RedirectToAction("Edit", new { id = propertyId });
            }

            var existingCount = await _context.PropertyImages
                .Where(i => i.PropertyId == propertyId)
                .CountAsync();

            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                    var imageUrl = await SaveImage(file);

                    _context.PropertyImages.Add(new PropertyImage
                    {
                        PropertyId = propertyId,
                        ImageUrl = imageUrl,
                        DisplayOrder = existingCount++
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{imageFiles.Count} image(s) uploaded to gallery.";
            return RedirectToAction("Edit", new { id = propertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.PropertyImages.FindAsync(id);
            if (image == null) return NotFound();

            var propertyId = image.PropertyId;

            // Delete file from disk
            if (!string.IsNullOrEmpty(image.ImageUrl) && image.ImageUrl.StartsWith("/image/"))
            {
                var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.PropertyImages.Remove(image);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Image removed from gallery.";
            return RedirectToAction("Edit", new { id = propertyId });
        }
    }
}
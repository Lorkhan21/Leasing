using Microsoft.AspNetCore.Identity;
using Leasing.Models;

namespace Leasing.Data
{
    public static class DbSeeder
    {
        // Seed properties
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Properties.Any())
                return;

            var properties = new List<Property>
            {
                new Property
                {
                    Title = "Galino Apartment",
                    Price = "₱14,000",
                    PriceSuffix = "– ₱22,000/mo",
                    Address = "Galino IV, Grace Park East, Caloocan, Metro Manila",
                    City = "Caloocan",
                    PropertyType = "Apartment",
                    Status = "For Rent",
                    IsFeatured = true,
                    AgentName = "Gerald Nacion",
                    AgentEmail = "Gerald21Nacion@gmail.com",
                    AgentImage = "/image/agent-1.jpg",
                    ImageUrl = "/image/Caloocan.png",
                    PostedDate = DateTime.Now.AddDays(-30),
                    Features = new List<PropertyFeature>
                    {
                        new() { Icon = "fa-bath", Label = "Bath", Value = "1" },
                        new() { Icon = "fa-ruler-combined", Label = "Area", Value = "33 sqm" }
                    }
                },
                new Property
                {
                    Title = "9TH AVE MOTORPOOL",
                    Price = "₱100,000",
                    PriceSuffix = "/MO",
                    Address = "31 9th Ave, Grace Park East, Caloocan",
                    City = "Caloocan",
                    PropertyType = "Warehouse",
                    Status = "For Rent",
                    IsFeatured = true,
                    AgentName = "Gerald Nacion",
                    AgentEmail = "Gerald21Nacion@gmail.com",
                    AgentImage = "/image/agent-1.jpg",
                    ImageUrl = "/image/Caloocan.png",
                    PostedDate = DateTime.Now.AddDays(-7),
                    Features = new List<PropertyFeature>
                    {
                        new() { Icon = "fa-bed", Label = "Bed", Value = "1" },
                        new() { Icon = "fa-bath", Label = "Baths", Value = "2" },
                        new() { Icon = "fa-ruler-combined", Label = "Area", Value = "336 sqm" }
                    }
                }
                // Add more seed properties if you want
            };

            context.Properties.AddRange(properties);
            context.SaveChanges();
        }

        // Seed roles and default admin user
        public static async Task SeedRolesAndAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 1. Create roles
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Create default admin user
            var adminEmail = "admin@leasing.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Create default regular user (optional)
            var regularEmail = "user@leasing.com";
            var regularUser = await userManager.FindByEmailAsync(regularEmail);

            if (regularUser == null)
            {
                regularUser = new ApplicationUser
                {
                    UserName = regularEmail,
                    Email = regularEmail,
                    FullName = "Regular User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(regularUser, "User@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }
        }
    }
}
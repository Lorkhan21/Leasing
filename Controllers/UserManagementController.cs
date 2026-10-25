using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Leasing.Data;

namespace Leasing.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /UserManagement
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // GET: /UserManagement/EditRole/{id}
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            ViewBag.CurrentRoles = currentRoles;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(user);
        }

        // POST: /UserManagement/EditRole/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role
            await _userManager.AddToRoleAsync(user, role);

            TempData["Success"] = $"Role for {user.Email} updated to {role}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /UserManagement/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Prevent deleting yourself
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            TempData["Success"] = $"User {user.Email} deleted.";
            return RedirectToAction(nameof(Index));
        }

        // NEW: GET Register
        public IActionResult Register()
        {
            return View();
        }

        // NEW: POST Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string email, string fullName, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email and password are required.";
                return View();
            }

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                TempData["Error"] = "A user with that email already exists.";
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var assignedRole = string.IsNullOrEmpty(role) ? "User" : role;
                await _userManager.AddToRoleAsync(user, assignedRole);

                TempData["Success"] = $"User {email} created successfully with role {assignedRole}.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return View();
        }
    }

}

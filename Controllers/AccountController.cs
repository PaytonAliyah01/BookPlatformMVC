using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookPlatformMVC.Areas.Identity.Data;

namespace BookPlatformMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        // Log out the user
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // Sign the user out
            return RedirectToAction("Index", "Home");  // Redirect to Home or any page you prefer
        }
    }
}

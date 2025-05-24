using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeeSe.Models.ViewModels;
using WeeSe.Services;

namespace WeeSe.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Se già autenticato, vai alla dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isValid = await _authService.ValidateUserAsync(model.Email, model.Password);
                
                if (isValid)
                {
                    var user = await _authService.GetUserAsync(model.Email);
                    
                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.sUtente),
                            new Claim(ClaimTypes.NameIdentifier, user.IDAnagraficaAccesso.ToString()),
                            new Claim("IDRuolo", user.IDRuolo.ToString()),
                            new Claim("IDAnagraficaAccesso", user.IDAnagraficaAccesso.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authProperties);

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                
                ModelState.AddModelError("", "Nome utente o password non validi");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Metodo temporaneo per testare la connessione al database
        [HttpGet]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                var authService = _authService as AuthService;
                if (authService != null)
                {
                    var result = await authService.TestDatabaseConnection();
                    return Json(new { success = true, message = result });
                }
                return Json(new { success = false, message = "Servizio non disponibile" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

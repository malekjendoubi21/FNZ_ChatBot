using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FNZ_ChatBot.Models;
using FNZ_ChatBot.Services;

namespace FNZ_ChatBot.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeService _verificationCodeService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailService emailService,
            IVerificationCodeService verificationCodeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _verificationCodeService = verificationCodeService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur connecté.");
                    
                    // Vérifier si l'utilisateur doit changer son mot de passe
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && user.MustChangePassword)
                    {
                        TempData["MustChangePassword"] = true;
                        return RedirectToAction("ChangePassword");
                    }
                    
                    return RedirectToLocal(returnUrl);
                }
                
                ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Vérifier si l'email existe
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Ne pas révéler que l'utilisateur n'existe pas pour des raisons de sécurité
                TempData["Message"] = "Si cette adresse email existe dans notre système, vous recevrez un code de vérification dans quelques minutes.";
                TempData["MessageType"] = "info";
                return View(model);
            }

            try
            {
                // Générer le code de vérification
                var verificationCode = _verificationCodeService.GenerateVerificationCode();
                
                // Sauvegarder le code en base
                var codeCreated = await _verificationCodeService.CreatePasswordResetCodeAsync(user.Id, verificationCode);
                
                if (!codeCreated)
                {
                    TempData["Error"] = "Une erreur s'est produite. Veuillez réessayer plus tard.";
                    return View(model);
                }

                // Envoyer l'email avec le code
                await _emailService.SendPasswordResetCodeAsync(user.Email!, user.FirstName, verificationCode);
                
                _logger.LogInformation("Code de réinitialisation envoyé pour l'utilisateur {Email}", model.Email);
                
                // Rediriger vers la page de réinitialisation avec l'email
                TempData["Email"] = model.Email;
                TempData["Success"] = "Un code de vérification a été envoyé à votre adresse email.";
                return RedirectToAction("ResetPassword");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du code de réinitialisation pour {Email}", model.Email);
                TempData["Error"] = "Une erreur s'est produite lors de l'envoi de l'email. Veuillez réessayer plus tard.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var model = new ResetPasswordViewModel();
            
            // Pré-remplir l'email si fourni
            if (TempData["Email"] is string email)
            {
                model.Email = email;
                TempData.Keep("Email"); // Garder pour le POST
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Adresse email non trouvée.");
                return View(model);
            }

            // Valider le code de vérification
            var isValidCode = await _verificationCodeService.ValidatePasswordResetCodeAsync(user.Id, model.VerificationCode);
            if (!isValidCode)
            {
                ModelState.AddModelError(nameof(model.VerificationCode), "Code de vérification invalide ou expiré.");
                return View(model);
            }

            try
            {
                // Réinitialiser le mot de passe
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                
                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Marquer le code comme utilisé
                await _verificationCodeService.MarkCodeAsUsedAsync(user.Id, model.VerificationCode);
                
                // Mettre à jour les propriétés de l'utilisateur
                user.MustChangePassword = false;
                user.LastPasswordChange = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Mot de passe réinitialisé avec succès pour l'utilisateur {Email}", model.Email);
                
                TempData["Success"] = "Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe pour {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Une erreur s'est produite. Veuillez réessayer plus tard.");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            var isFirstLogin = TempData["MustChangePassword"] as bool? ?? false;
            var model = new ChangePasswordViewModel
            {
                IsFirstLogin = isFirstLogin
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Utilisateur introuvable.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Mettre à jour les propriétés de l'utilisateur
            user.MustChangePassword = false;
            user.LastPasswordChange = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("L'utilisateur a changé son mot de passe avec succès.");

            TempData["Success"] = "Votre mot de passe a été modifié avec succès.";
            
            if (model.IsFirstLogin)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utilisateur déconnecté.");
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Chat");
            }
        }
    }
}
using Microsoft.AspNetCore.Identity;
using FNZ_ChatBot.Models;

namespace FNZ_ChatBot.Middleware
{
    public class ForcePasswordChangeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ForcePasswordChangeMiddleware> _logger;

        public ForcePasswordChangeMiddleware(RequestDelegate next, ILogger<ForcePasswordChangeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            // Exclure certaines routes de la vérification
            var path = context.Request.Path.ToString().ToLower();
            var excludedPaths = new[]
            {
                "/account/login",
                "/account/logout", 
                "/account/changepassword",
                "/css/",
                "/js/",
                "/lib/",
                "/images/",
                "/favicon.ico"
            };

            if (excludedPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // Vérifier si l'utilisateur est connecté
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null && user.MustChangePassword)
                {
                    // Rediriger vers la page de changement de mot de passe si ce n'est pas déjà la destination
                    if (!path.Contains("/account/changepassword"))
                    {
                        _logger.LogInformation("Redirection de {User} vers le changement de mot de passe obligatoire", user.Email);
                        context.Response.Redirect("/Account/ChangePassword");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method pour faciliter l'enregistrement
    public static class ForcePasswordChangeMiddlewareExtensions
    {
        public static IApplicationBuilder UseForcePasswordChange(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForcePasswordChangeMiddleware>();
        }
    }
}
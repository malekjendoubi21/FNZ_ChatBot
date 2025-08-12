using Microsoft.AspNetCore.Identity;
using FNZ_ChatBot.Models;
using FNZ_ChatBot.Services;
using FNZ_ChatBot.Data;
using Microsoft.EntityFrameworkCore;

namespace FNZ_ChatBot.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Cr�er les r�les
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Cr�er un administrateur par d�faut
            var adminEmail = "admin@fnz.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "FNZ",
                    IsAdmin = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Migrer les donn�es de data.json vers la base de donn�es
            await MigrateKnowledgeBase(context, adminUser.Id);
        }

        private static async Task MigrateKnowledgeBase(ApplicationDbContext context, string adminUserId)
        {
            // V�rifier si la base de connaissances existe d�j�
            if (await context.KnowledgeBase.AnyAsync())
            {
                return; // Les donn�es existent d�j�
            }

            try
            {
                // Charger les donn�es depuis data.json
                var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
                if (File.Exists(jsonFilePath))
                {
                    var messages = JsonLoader.LoadMessages(jsonFilePath);
                    
                    foreach (var message in messages)
                    {
                        var knowledgeItem = new KnowledgeBase
                        {
                            Question = message.Question,
                            Response = message.Response,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            CreatedBy = adminUserId,
                            ModifiedBy = adminUserId,
                            IsActive = true
                        };
                        
                        context.KnowledgeBase.Add(knowledgeItem);
                    }
                    
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log l'erreur mais ne pas faire �chouer l'initialisation
                Console.WriteLine($"Erreur lors de la migration de la base de connaissances: {ex.Message}");
            }
        }
    }
}
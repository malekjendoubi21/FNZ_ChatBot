using FNZ_ChatBot.Data;
using FNZ_ChatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace FNZ_ChatBot.Services
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VerificationCodeService> _logger;

        public VerificationCodeService(ApplicationDbContext context, ILogger<VerificationCodeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Code à 6 chiffres
        }

        public async Task<bool> CreatePasswordResetCodeAsync(string userId, string code)
        {
            try
            {
                // Invalider les anciens codes non utilisés pour cet utilisateur
                var existingCodes = await _context.PasswordResetCodes
                    .Where(p => p.UserId == userId && !p.IsUsed)
                    .ToListAsync();

                foreach (var existingCode in existingCodes)
                {
                    existingCode.IsUsed = true;
                    existingCode.UsedAt = DateTime.UtcNow;
                }

                // Créer le nouveau code
                var resetCode = new PasswordResetCode
                {
                    UserId = userId,
                    Code = code,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Expire dans 15 minutes
                    IsUsed = false
                };

                _context.PasswordResetCodes.Add(resetCode);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Code de réinitialisation créé pour l'utilisateur {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la création du code pour l'utilisateur {userId}");
                return false;
            }
        }

        public async Task<bool> ValidatePasswordResetCodeAsync(string userId, string code)
        {
            try
            {
                var resetCode = await _context.PasswordResetCodes
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.Code == code && !p.IsUsed);

                if (resetCode == null)
                {
                    _logger.LogWarning($"Code invalide ou déjà utilisé pour l'utilisateur {userId}");
                    return false;
                }

                if (DateTime.UtcNow > resetCode.ExpiresAt)
                {
                    _logger.LogWarning($"Code expiré pour l'utilisateur {userId}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la validation du code pour l'utilisateur {userId}");
                return false;
            }
        }

        public async Task<bool> MarkCodeAsUsedAsync(string userId, string code)
        {
            try
            {
                var resetCode = await _context.PasswordResetCodes
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.Code == code && !p.IsUsed);

                if (resetCode == null)
                {
                    return false;
                }

                resetCode.IsUsed = true;
                resetCode.UsedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Code marqué comme utilisé pour l'utilisateur {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du marquage du code comme utilisé pour l'utilisateur {userId}");
                return false;
            }
        }

        public async Task CleanupExpiredCodesAsync()
        {
            try
            {
                var expiredCodes = await _context.PasswordResetCodes
                    .Where(p => p.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredCodes.Any())
                {
                    _context.PasswordResetCodes.RemoveRange(expiredCodes);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Suppression de {expiredCodes.Count} codes expirés");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du nettoyage des codes expirés");
            }
        }
    }
}
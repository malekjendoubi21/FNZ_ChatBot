namespace FNZ_ChatBot.Services
{
    public interface IVerificationCodeService
    {
        string GenerateVerificationCode();
        Task<bool> CreatePasswordResetCodeAsync(string userId, string code);
        Task<bool> ValidatePasswordResetCodeAsync(string userId, string code);
        Task<bool> MarkCodeAsUsedAsync(string userId, string code);
        Task CleanupExpiredCodesAsync();
    }
}
using MailKit.Net.Smtp;
using MimeKit;

namespace FNZ_ChatBot.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendNewUserCredentialsAsync(string toEmail, string firstName, string lastName, string tempPassword);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:FromName"] ?? "FNZ ChatBot",
                    _configuration["EmailSettings:FromEmail"] ?? "malekeljendoubi@gmail.com"
                ));
                
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Pour développement, on simule l'envoi
                if (bool.TryParse(_configuration["EmailSettings:UseRealSmtp"], out var useReal) && useReal)
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        int.Parse(_configuration["EmailSettings:Port"] ?? "587"),
                        MailKit.Security.SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    
                    _logger.LogInformation("Email envoyé avec succès à {Email}", toEmail);
                }
                else
                {
                    // Mode développement - log l'email au lieu de l'envoyer
                    _logger.LogInformation("EMAIL SIMULÉ - À: {Email}, Sujet: {Subject}, Contenu: {Content}", 
                        toEmail, subject, htmlMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {Email}", toEmail);
                throw;
            }
        }

        public async Task SendNewUserCredentialsAsync(string toEmail, string firstName, string lastName, string tempPassword)
        {
            var subject = "Bienvenue sur FNZ ChatBot - Vos identifiants de connexion";
            
            var htmlMessage = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f8f9fa; }}
                        .credentials {{ background-color: white; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0; }}
                        .warning {{ background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }}
                        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🤖 Bienvenue sur FNZ ChatBot</h1>
                        </div>
                        
                        <div class='content'>
                            <h2>Bonjour {firstName} {lastName},</h2>
                            
                            <p>Votre compte utilisateur a été créé avec succès par un administrateur. Voici vos identifiants de connexion :</p>
                            
                            <div class='credentials'>
                                <h3>Vos identifiants :</h3>
                                <p><strong>Email :</strong> {toEmail}</p>
                                <p><strong>Mot de passe temporaire :</strong> <code>{tempPassword}</code></p>
                            </div>
                            
                            <div class='warning'>
                                <h3>⚠️ Important :</h3>
                                <p>Pour des raisons de sécurité, vous devrez <strong>obligatoirement changer votre mot de passe</strong> lors de votre première connexion.</p>
                            </div>
                            
                            <h3>Comment vous connecter :</h3>
                            <ol>
                                <li>Rendez-vous sur la page de connexion du FNZ ChatBot</li>
                                <li>Utilisez votre email et le mot de passe temporaire ci-dessus</li>
                                <li>Suivez les instructions pour définir un nouveau mot de passe sécurisé</li>
                                <li>Commencez à utiliser votre assistant virtuel intelligent !</li>
                            </ol>
                            
                            <p>Si vous avez des questions ou rencontrez des difficultés, n'hésitez pas à contacter votre administrateur.</p>
                            
                            <p>Cordialement,<br>L'équipe FNZ ChatBot</p>
                        </div>
                        
                        <div class='footer'>
                            <p>Cet email a été généré automatiquement. Merci de ne pas répondre à cette adresse.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlMessage);
        }
    }
}
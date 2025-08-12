using System.Security.Cryptography;
using System.Text;

namespace FNZ_ChatBot.Services
{
    public interface IPasswordGeneratorService
    {
        string GenerateSecurePassword(int length = 12);
    }

    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericChars = "0123456789";
        private const string SpecialChars = "!@#$%&*";

        public string GenerateSecurePassword(int length = 12)
        {
            if (length < 8)
                length = 8; // Minimum sécurisé

            var allChars = LowerCaseChars + UpperCaseChars + NumericChars + SpecialChars;
            var password = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                // S'assurer qu'il y a au moins un caractère de chaque type
                password.Append(GetRandomChar(LowerCaseChars, rng));
                password.Append(GetRandomChar(UpperCaseChars, rng));
                password.Append(GetRandomChar(NumericChars, rng));
                password.Append(GetRandomChar(SpecialChars, rng));

                // Remplir le reste avec des caractères aléatoires
                for (int i = 4; i < length; i++)
                {
                    password.Append(GetRandomChar(allChars, rng));
                }
            }

            // Mélanger les caractères pour éviter un pattern prévisible
            return ShuffleString(password.ToString());
        }

        private char GetRandomChar(string chars, RandomNumberGenerator rng)
        {
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            var randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % chars.Length;
            return chars[randomIndex];
        }

        private string ShuffleString(string input)
        {
            var array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    var randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    var j = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);
                    
                    // Échanger array[i] et array[j]
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
            return new string(array);
        }
    }
}
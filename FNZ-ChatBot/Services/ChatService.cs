using FNZ_ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FNZ_ChatBot.Services

{
    public class ChatService : IChatService
    {
        private readonly List<Message> _messages;

        public ChatService(string jsonFilePath)
        {
            _messages = JsonLoader.LoadMessages(jsonFilePath);
        }

        /* public string GetResponse(string userInput)
         {
             var message = _messages
                 .FirstOrDefault(m => string.Equals(m.Question.Trim(), userInput.Trim(), StringComparison.OrdinalIgnoreCase));

             return message?.Response ?? "Désolé, je ne connais pas encore la réponse à cette question.";
         }*/
        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Veuillez entrer une question.";

            // Normalisation
            var input = NormalizeText(userInput);

            // Liste des salutations (français + anglais)
            var greetings = new HashSet<string>
    {
        "bonjour", "bonsoir", "salut", "hello", "hi", "hey", "coucou", "yo"
    };

            // Si l'entrée contient uniquement une salutation
            if (greetings.Any(g => input.Contains(g)))
                return "Bonjour ! Comment puis-je vous aider aujourd'hui ?";

            // Liste des mots à ignorer
            var stopWords = new HashSet<string>
    {
        "comment", "quelle", "quelles", "quels", "quel", "que", "quoi",
        "est", "sont", "les", "le", "la", "de", "du", "des", "un", "une",
        "pour", "dans", "avec", "à", "au", "aux", "en", "et", "sur", "ou"
    };

            // Découpage et suppression des stop-words
            var inputWords = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !stopWords.Contains(w))
                .ToList();

            if (!inputWords.Any())
                return "Pouvez-vous préciser votre question ?";

            // Scorer les questions
            var scoredMessages = _messages
                .Select(m => new
                {
                    Message = m,
                    Score = inputWords.Count(word => NormalizeText(m.Question).Contains(word))
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();

            if (!scoredMessages.Any())
                return "Désolé, je ne connais pas encore la réponse à cette question.";

            // Si le meilleur match est assez bon
            var bestMatch = scoredMessages.First();
            if (bestMatch.Score >= 2)
                return bestMatch.Message.Response;

            // Sinon proposer les questions proches
            var suggestions = scoredMessages.Take(5).Select(x => x.Message.Question).ToList();
            return "Je n'ai pas trouvé de réponse exacte. Voici des questions proches :\n- " + string.Join("\n- ", suggestions);
        }

        private string NormalizeText(string text)
        {
            text = text.ToLowerInvariant();

            // Supprimer les accents (é → e, ç → c, etc.)
            text = text.Normalize(NormalizationForm.FormD);
            text = new string(text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            // Supprimer ponctuation
            text = Regex.Replace(text, @"[^\w\s]", " ");

            // Supprimer espaces multiples
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }


    }
}

using FNZ_ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace FNZ_ChatBot.Services

{
    public class ChatService : IChatService
    {
        private readonly List<Message> _messages;

        public ChatService(string jsonFilePath)
        {
            _messages = JsonLoader.LoadMessages(jsonFilePath);
        }

        public string GetResponse(string userInput)
        {
            var message = _messages
                .FirstOrDefault(m => string.Equals(m.Question.Trim(), userInput.Trim(), StringComparison.OrdinalIgnoreCase));

            return message?.Response ?? "Désolé, je ne connais pas encore la réponse à cette question.";
        }
    }
}

using FNZ_ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FNZ_ChatBot.Services
{
    class JsonLoader
    {
        public static List<Message> LoadMessages(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Le fichier {filePath} est introuvable.");

            string json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Message>>(json, options) ?? new List<Message>();
        }
    }
}

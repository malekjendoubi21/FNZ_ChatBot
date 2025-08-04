using FNZ_ChatBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FNZ_ChatBot.Services
{
    class JsonLoader
    {
        public static List<Message> LoadMessages(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Le fichier {filePath} est introuvable.");

            // Forcer l'encodage UTF‑8
            string json = File.ReadAllText(filePath, Encoding.UTF8);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                return JsonSerializer.Deserialize<List<Message>>(json, options) ?? new List<Message>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Erreur lors du chargement du fichier JSON {filePath}: {ex.Message}");
            }
        }
    }
}

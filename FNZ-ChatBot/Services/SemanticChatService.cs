using FNZ_ChatBot.Models;
using FNZ_ChatBot.Services;
using FNZ_ChatBot.Data;
using Microsoft.EntityFrameworkCore;

public class SemanticChatService : IChatService
{
    private readonly EmbeddingService _embeddingService;
    private readonly ApplicationDbContext _context;

    public SemanticChatService(string modelPath, ApplicationDbContext context)
    {
        _embeddingService = new EmbeddingService(modelPath);
        _context = context;
    }

    public string GetResponse(string userInput)
    {
        return GetResponseAsync(userInput).Result;
    }

    public async Task<string> GetResponseAsync(string userInput, string? userId = null, int? conversationId = null)
    {
        var response = await GetResponseInternal(userInput);
        
        // Enregistrer dans l'historique si un utilisateur est connecté
        if (!string.IsNullOrEmpty(userId))
        {
            var conversationHistory = new ConversationHistory
            {
                UserId = userId,
                ConversationId = conversationId,
                Question = userInput,
                Response = response,
                CreatedDate = DateTime.UtcNow
            };

            _context.ConversationHistory.Add(conversationHistory);
            await _context.SaveChangesAsync();
        }

        return response;
    }

    private async Task<string> GetResponseInternal(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return "Veuillez entrer une question.";

        // Gestion des salutations
        var greetings = new[] { "bonjour", "salut", "hello", "hey", "coucou" };
        if (greetings.Any(g => userInput.ToLower().Contains(g)))
            return "Bonjour ! Comment puis-je vous aider ?";

        // Charger les données actives de la base de connaissances
        var knowledgeItems = await _context.KnowledgeBase
            .Where(k => k.IsActive)
            .ToListAsync();

        if (!knowledgeItems.Any())
        {
            return "La base de connaissances est vide. Veuillez contacter un administrateur.";
        }

        // Créer les embeddings pour la recherche sémantique
        var inputEmbedding = _embeddingService.GetEmbedding(userInput);
        
        var bestMatch = knowledgeItems
            .Select(k => new
            {
                k.Response,
                Score = EmbeddingService.CosineSimilarity(inputEmbedding, _embeddingService.GetEmbedding(k.Question))
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (bestMatch == null || bestMatch.Score < 0.6f)
            return "Désolé, je n'ai pas trouvé de réponse pertinente dans la base de connaissances.";

        return bestMatch.Response;
    }
}

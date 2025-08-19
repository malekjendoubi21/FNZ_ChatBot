using FNZ_ChatBot.Models;
using FNZ_ChatBot.Data;
using Microsoft.EntityFrameworkCore;

namespace FNZ_ChatBot.Services
{
    /// <summary>
    /// Service pour améliorer la recherche sémantique en enrichissant la base de connaissances
    /// </summary>
    public class KnowledgeEnhancementService
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeEnhancementService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Enrichit une question avec des variations et des synonymes pour améliorer la recherche
        /// </summary>
        public async Task<bool> EnrichKnowledgeBaseAsync()
        {
            try
            {
                var knowledgeItems = await _context.KnowledgeBase
                    .Where(k => k.IsActive)
                    .ToListAsync();

                foreach (var item in knowledgeItems)
                {
                    var enrichedQuestion = EnrichQuestion(item.Question);
                    if (enrichedQuestion != item.Question)
                    {
                        // Optionnel : créer des entrées supplémentaires ou enrichir la question existante
                        // Pour cet exemple, on peut ajouter les variations comme commentaire dans ModifiedBy
                        item.ModifiedBy += $" [Enrichi: {enrichedQuestion}]";
                        item.ModifiedDate = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'enrichissement : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Enrichit une question avec des variations et synonymes
        /// </summary>
        private string EnrichQuestion(string originalQuestion)
        {
            var enriched = originalQuestion;
            
            // Dictionnaire de synonymes et variations courantes
            var synonyms = new Dictionary<string, string[]>
            {
                ["comment"] = new[] { "comment", "de quelle manière", "comment faire", "comment procéder" },
                ["quoi"] = new[] { "quoi", "que", "qu'est-ce que", "quel" },
                ["pourquoi"] = new[] { "pourquoi", "pour quelle raison", "dans quel but" },
                ["où"] = new[] { "où", "dans quel endroit", "à quel endroit" },
                ["quand"] = new[] { "quand", "à quel moment", "dans quel délai" },
                ["qui"] = new[] { "qui", "quelle personne", "quel responsable" },
                ["prix"] = new[] { "prix", "coût", "tarif", "montant", "frais" },
                ["service"] = new[] { "service", "prestation", "offre", "solution" },
                ["client"] = new[] { "client", "utilisateur", "usager", "bénéficiaire" },
                ["problème"] = new[] { "problème", "souci", "difficulté", "bug", "erreur" },
                ["aide"] = new[] { "aide", "assistance", "support", "soutien" },
                ["fonctionnement"] = new[] { "fonctionnement", "marche", "opération", "utilisation" },
            };

            // Ajouter des variations de formulation
            var variations = new List<string> { enriched };

            // Ajouter des formes de politesse
            if (!enriched.ToLower().Contains("pouvez-vous") && !enriched.ToLower().Contains("pourriez-vous"))
            {
                variations.Add($"Pouvez-vous m'expliquer {enriched.ToLower()}");
                variations.Add($"Pourriez-vous me dire {enriched.ToLower()}");
            }

            // Ajouter des formes interrogatives
            if (!enriched.Contains("?"))
            {
                variations.Add(enriched + " ?");
            }

            return string.Join(" | ", variations.Distinct());
        }

        /// <summary>
        /// Analyse la performance des recherches pour suggérer des améliorations
        /// </summary>
        public async Task<List<string>> AnalyzeSearchPerformanceAsync()
        {
            var suggestions = new List<string>();

            try
            {
                // Analyser les questions sans réponses dans l'historique
                var unansweredQuestions = await _context.ConversationHistory
                    .Where(h => h.Response.Contains("n'ai pas trouvé") || 
                               h.Response.Contains("dépasse mes connaissances"))
                    .GroupBy(h => h.Question.ToLower())
                    .Select(g => new { Question = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                foreach (var item in unansweredQuestions)
                {
                    suggestions.Add($"Question fréquemment sans réponse ({item.Count}x): {item.Question}");
                }

                // Analyser les questions courtes qui pourraient bénéficier de plus de contexte
                var shortQuestions = await _context.KnowledgeBase
                    .Where(k => k.IsActive && k.Question.Length < 20)
                    .Select(k => k.Question)
                    .ToListAsync();

                if (shortQuestions.Any())
                {
                    suggestions.Add($"Questions courtes à enrichir: {string.Join(", ", shortQuestions.Take(5))}");
                }

                return suggestions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'analyse : {ex.Message}");
                return new List<string> { "Erreur lors de l'analyse des performances" };
            }
        }

        /// <summary>
        /// Suggère des améliorations pour une question spécifique
        /// </summary>
        public List<string> SuggestImprovements(string question)
        {
            var suggestions = new List<string>();

            // Vérifier la longueur
            if (question.Length < 10)
            {
                suggestions.Add("La question est très courte, considérez l'enrichir avec plus de contexte");
            }

            // Vérifier la présence de mots-clés importants
            var importantKeywords = new[] { "comment", "pourquoi", "quand", "où", "qui", "quoi" };
            if (!importantKeywords.Any(k => question.ToLower().Contains(k)))
            {
                suggestions.Add("Considérez ajouter des mots interrogatifs (comment, pourquoi, etc.)");
            }

            // Vérifier la ponctuation
            if (!question.Contains("?") && !question.Contains("."))
            {
                suggestions.Add("Ajoutez une ponctuation appropriée (? ou .)");
            }

            return suggestions;
        }
    }
}
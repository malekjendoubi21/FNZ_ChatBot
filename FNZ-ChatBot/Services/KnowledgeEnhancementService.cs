using FNZ_ChatBot.Models;
using FNZ_ChatBot.Data;
using Microsoft.EntityFrameworkCore;

namespace FNZ_ChatBot.Services
{
    /// <summary>
    /// Service pour am�liorer la recherche s�mantique en enrichissant la base de connaissances
    /// </summary>
    public class KnowledgeEnhancementService
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeEnhancementService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Enrichit une question avec des variations et des synonymes pour am�liorer la recherche
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
                        // Optionnel : cr�er des entr�es suppl�mentaires ou enrichir la question existante
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
                ["comment"] = new[] { "comment", "de quelle mani�re", "comment faire", "comment proc�der" },
                ["quoi"] = new[] { "quoi", "que", "qu'est-ce que", "quel" },
                ["pourquoi"] = new[] { "pourquoi", "pour quelle raison", "dans quel but" },
                ["o�"] = new[] { "o�", "dans quel endroit", "� quel endroit" },
                ["quand"] = new[] { "quand", "� quel moment", "dans quel d�lai" },
                ["qui"] = new[] { "qui", "quelle personne", "quel responsable" },
                ["prix"] = new[] { "prix", "co�t", "tarif", "montant", "frais" },
                ["service"] = new[] { "service", "prestation", "offre", "solution" },
                ["client"] = new[] { "client", "utilisateur", "usager", "b�n�ficiaire" },
                ["probl�me"] = new[] { "probl�me", "souci", "difficult�", "bug", "erreur" },
                ["aide"] = new[] { "aide", "assistance", "support", "soutien" },
                ["fonctionnement"] = new[] { "fonctionnement", "marche", "op�ration", "utilisation" },
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
        /// Analyse la performance des recherches pour sugg�rer des am�liorations
        /// </summary>
        public async Task<List<string>> AnalyzeSearchPerformanceAsync()
        {
            var suggestions = new List<string>();

            try
            {
                // Analyser les questions sans r�ponses dans l'historique
                var unansweredQuestions = await _context.ConversationHistory
                    .Where(h => h.Response.Contains("n'ai pas trouv�") || 
                               h.Response.Contains("d�passe mes connaissances"))
                    .GroupBy(h => h.Question.ToLower())
                    .Select(g => new { Question = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                foreach (var item in unansweredQuestions)
                {
                    suggestions.Add($"Question fr�quemment sans r�ponse ({item.Count}x): {item.Question}");
                }

                // Analyser les questions courtes qui pourraient b�n�ficier de plus de contexte
                var shortQuestions = await _context.KnowledgeBase
                    .Where(k => k.IsActive && k.Question.Length < 20)
                    .Select(k => k.Question)
                    .ToListAsync();

                if (shortQuestions.Any())
                {
                    suggestions.Add($"Questions courtes � enrichir: {string.Join(", ", shortQuestions.Take(5))}");
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
        /// Sugg�re des am�liorations pour une question sp�cifique
        /// </summary>
        public List<string> SuggestImprovements(string question)
        {
            var suggestions = new List<string>();

            // V�rifier la longueur
            if (question.Length < 10)
            {
                suggestions.Add("La question est tr�s courte, consid�rez l'enrichir avec plus de contexte");
            }

            // V�rifier la pr�sence de mots-cl�s importants
            var importantKeywords = new[] { "comment", "pourquoi", "quand", "o�", "qui", "quoi" };
            if (!importantKeywords.Any(k => question.ToLower().Contains(k)))
            {
                suggestions.Add("Consid�rez ajouter des mots interrogatifs (comment, pourquoi, etc.)");
            }

            // V�rifier la ponctuation
            if (!question.Contains("?") && !question.Contains("."))
            {
                suggestions.Add("Ajoutez une ponctuation appropri�e (? ou .)");
            }

            return suggestions;
        }
    }
}
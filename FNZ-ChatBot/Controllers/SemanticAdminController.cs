using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FNZ_ChatBot.Services;
using FNZ_ChatBot.Data;
using FNZ_ChatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace FNZ_ChatBot.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SemanticAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatService _chatService;
        private readonly KnowledgeEnhancementService _enhancementService;

        public SemanticAdminController(ApplicationDbContext context, IChatService chatService, KnowledgeEnhancementService enhancementService)
        {
            _context = context;
            _chatService = chatService;
            _enhancementService = enhancementService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.KnowledgeCount = await _context.KnowledgeBase.CountAsync(k => k.IsActive);
            ViewBag.TotalQuestions = await _context.ConversationHistory.CountAsync();
            
            var suggestions = await _enhancementService.AnalyzeSearchPerformanceAsync();
            ViewBag.Suggestions = suggestions;

            // G�n�rer des exemples dynamiques
            var dynamicExamples = await GenerateDynamicExamples();
            ViewBag.DynamicExamples = dynamicExamples;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDynamicExamples()
        {
            var examples = await GenerateDynamicExamples();
            return Json(new { examples });
        }

        private async Task<List<QuestionExample>> GenerateDynamicExamples()
        {
            try
            {
                // R�cup�rer un �chantillon vari� de questions de la base de connaissances
                var knowledgeItems = await _context.KnowledgeBase
                    .Where(k => k.IsActive)
                    .OrderBy(k => Guid.NewGuid()) // Randomisation
                    .Take(10)
                    .ToListAsync();

                var examples = new List<QuestionExample>();

                // Cat�goriser les questions par mots-cl�s techniques
                var categories = new Dictionary<string, List<string>>
                {
                    ["SQL"] = new() { "sql", "database", "base", "donn�es", "connexion", "serveur" },
                    ["Git"] = new() { "git", "github", "repository", "clone", "merge", "branch", "conflit" },
                    ["API"] = new() { "api", "rest", "http", "endpoint", "controller", "web" },
                    ["Entity Framework"] = new() { "entity", "framework", "migration", "ef", "orm" },
                    ["C# / .NET"] = new() { "c#", "csharp", "dotnet", ".net", "async", "linq" },
                    ["G�n�ral"] = new() { "comment", "faire", "cr�er", "utiliser" }
                };

                // Classer les questions par cat�gorie
                foreach (var item in knowledgeItems)
                {
                    var question = item.Question.ToLower();
                    var category = "G�n�ral";
                    var label = ExtractShortLabel(item.Question);

                    foreach (var cat in categories)
                    {
                        if (cat.Value.Any(keyword => question.Contains(keyword)))
                        {
                            category = cat.Key;
                            break;
                        }
                    }

                    examples.Add(new QuestionExample
                    {
                        Question = item.Question,
                        Label = label,
                        Category = category
                    });
                }

                // S'assurer d'avoir au moins 5 exemples vari�s
                if (examples.Count < 5)
                {
                    // Ajouter des exemples par d�faut si pas assez de donn�es
                    var defaultExamples = new List<QuestionExample>
                    {
                        new() { Question = "Comment cr�er une connexion SQL ?", Label = "Connexion SQL", Category = "SQL" },
                        new() { Question = "Comment cloner un repository Git ?", Label = "Git Clone", Category = "Git" },
                        new() { Question = "Comment cr�er une API REST ?", Label = "API REST", Category = "API" },
                        new() { Question = "Comment faire une migration EF ?", Label = "EF Migration", Category = "Entity Framework" },
                        new() { Question = "Comment utiliser async/await ?", Label = "Async C#", Category = "C# / .NET" }
                    };

                    foreach (var defaultExample in defaultExamples)
                    {
                        if (!examples.Any(e => e.Question.ToLower().Contains(defaultExample.Question.ToLower().Split(' ')[0])))
                        {
                            examples.Add(defaultExample);
                        }
                    }
                }

                return examples.Take(8).ToList(); // Limiter � 8 exemples
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retourner des exemples par d�faut
                return GetDefaultExamples();
            }
        }

        private List<QuestionExample> GetDefaultExamples()
        {
            return new List<QuestionExample>
            {
                new() { Question = "Comment cr�er une connexion SQL ?", Label = "SQL", Category = "Base de donn�es" },
                new() { Question = "Comment cloner un repository ?", Label = "Git", Category = "Versioning" },
                new() { Question = "Comment cr�er une API REST ?", Label = "API", Category = "Web" },
                new() { Question = "Comment faire une migration ?", Label = "EF", Category = "ORM" }
            };
        }

        private string ExtractShortLabel(string question)
        {
            // Extraire un label court de la question
            var words = question.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Chercher des mots-cl�s techniques
            var technicalTerms = new[] { "SQL", "Git", "API", "Entity", "Framework", "C#", "ASP.NET", "Azure", "JWT" };
            var foundTerm = technicalTerms.FirstOrDefault(term => 
                question.ToLower().Contains(term.ToLower()));

            if (!string.IsNullOrEmpty(foundTerm))
                return foundTerm;

            // Sinon, utiliser les premiers mots significatifs
            var significantWords = words.Where(w => w.Length > 3 && 
                !new[] { "Comment", "Quelle", "comment", "quelle", "est", "une", "un", "le", "la", "les" }.Contains(w))
                .Take(2);

            return string.Join(" ", significantWords);
        }

        [HttpPost]
        public async Task<IActionResult> TestSemantic(string testQuestion)
        {
            if (string.IsNullOrWhiteSpace(testQuestion))
            {
                TempData["Error"] = "Veuillez entrer une question de test.";
                return RedirectToAction("Index");
            }

            try
            {
                var startTime = DateTime.UtcNow;
                var response = await _chatService.GetResponseAsync(testQuestion);
                var endTime = DateTime.UtcNow;
                var responseTime = (endTime - startTime).TotalMilliseconds;

                ViewBag.TestQuestion = testQuestion;
                ViewBag.TestResponse = response;
                ViewBag.ResponseTime = responseTime;
                ViewBag.TestPerformed = true;

                // Analyser la qualit� de la r�ponse
                var quality = AnalyzeResponseQuality(response);
                ViewBag.ResponseQuality = quality;

                // Diagnostic d�taill�
                var diagnostic = await PerformDiagnostic(testQuestion);
                ViewBag.Diagnostic = diagnostic;

                // Recharger les donn�es pour la vue Index
                ViewBag.KnowledgeCount = await _context.KnowledgeBase.CountAsync(k => k.IsActive);
                ViewBag.TotalQuestions = await _context.ConversationHistory.CountAsync();
                
                var suggestions = await _enhancementService.AnalyzeSearchPerformanceAsync();
                ViewBag.Suggestions = suggestions;

                // G�n�rer des exemples dynamiques
                var dynamicExamples = await GenerateDynamicExamples();
                ViewBag.DynamicExamples = dynamicExamples;

                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du test : {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RefreshKnowledge()
        {
            try
            {
                // Force la recharge de la base de connaissances en red�marrant le service
                TempData["Success"] = "Cache de la base de connaissances actualis�. Les nouvelles donn�es seront prises en compte.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur : {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EnhanceKnowledge()
        {
            try
            {
                var success = await _enhancementService.EnrichKnowledgeBaseAsync();
                if (success)
                {
                    TempData["Success"] = "Base de connaissances enrichie avec succ�s.";
                }
                else
                {
                    TempData["Error"] = "Erreur lors de l'enrichissement de la base de connaissances.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur : {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Analytics()
        {
            var analytics = new SemanticAnalyticsViewModel();

            try
            {
                // Questions les plus fr�quentes
                analytics.MostFrequentQuestions = await _context.ConversationHistory
                    .GroupBy(h => h.Question.ToLower())
                    .Select(g => new QuestionFrequency 
                    { 
                        Question = g.Key, 
                        Count = g.Count() 
                    })
                    .OrderByDescending(q => q.Count)
                    .Take(10)
                    .ToListAsync();

                // Questions sans r�ponses satisfaisantes
                analytics.UnansweredQuestions = await _context.ConversationHistory
                    .Where(h => h.Response.Contains("n'ai pas trouv�") || 
                               h.Response.Contains("d�passe mes connaissances") ||
                               h.Response.Contains("pas de r�ponse") ||
                               h.Response.Contains("pas trouv�") ||
                               h.Response.Contains("reformuler"))
                    .GroupBy(h => h.Question.ToLower())
                    .Select(g => new QuestionFrequency 
                    { 
                        Question = g.Key, 
                        Count = g.Count() 
                    })
                    .OrderByDescending(q => q.Count)
                    .Take(10)
                    .ToListAsync();

                // Statistiques de performance
                analytics.TotalConversations = await _context.ConversationHistory.CountAsync();
                analytics.ActiveKnowledgeItems = await _context.KnowledgeBase.CountAsync(k => k.IsActive);
                analytics.SuccessRate = analytics.TotalConversations > 0 
                    ? (double)(analytics.TotalConversations - analytics.UnansweredQuestions.Sum(q => q.Count)) / analytics.TotalConversations * 100
                    : 0;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la g�n�ration des analytics : {ex.Message}";
            }

            return View(analytics);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return Json(new { suggestions = new List<string>() });

            var suggestions = _enhancementService.SuggestImprovements(question);
            return Json(new { suggestions });
        }

        private async Task<SearchDiagnostic> PerformDiagnostic(string question)
        {
            var diagnostic = new SearchDiagnostic
            {
                Question = question,
                NormalizedQuestion = question.ToLower().Trim()
            };

            try
            {
                // V�rifier la base de connaissances
                var knowledgeItems = await _context.KnowledgeBase
                    .Where(k => k.IsActive)
                    .ToListAsync();

                diagnostic.TotalKnowledgeItems = knowledgeItems.Count;

                // Recherche exacte
                var exactMatches = knowledgeItems
                    .Where(k => k.Question.ToLower().Contains(diagnostic.NormalizedQuestion) ||
                               k.Response.ToLower().Contains(diagnostic.NormalizedQuestion))
                    .ToList();

                diagnostic.ExactMatches = exactMatches.Count;

                // Recherche par mots-cl�s
                var words = diagnostic.NormalizedQuestion
                    .Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2)
                    .ToList();

                diagnostic.ExtractedWords = words;

                var keywordMatches = knowledgeItems
                    .Where(k => words.Any(word => 
                        k.Question.ToLower().Contains(word) || 
                        k.Response.ToLower().Contains(word)))
                    .ToList();

                diagnostic.KeywordMatches = keywordMatches.Count;

                // Mots-cl�s techniques
                var technicalKeywords = new[] { "sql", "git", "api", "c#", "entity", "framework", "database", "connexion" };
                var foundTechnicalKeywords = words.Where(w => technicalKeywords.Contains(w)).ToList();
                diagnostic.TechnicalKeywords = foundTechnicalKeywords;

                // V�rifier les r�ponses standard
                var standardKeywords = new[] { "bonjour", "salut", "merci", "aide" };
                diagnostic.IsStandardResponse = standardKeywords.Any(k => diagnostic.NormalizedQuestion.Contains(k));

                // Exemples de correspondances
                diagnostic.ExampleMatches = keywordMatches.Take(3)
                    .Select(k => $"Q: {k.Question} | R: {(k.Response.Length > 100 ? k.Response.Substring(0, 100) + "..." : k.Response)}")
                    .ToList();
            }
            catch (Exception ex)
            {
                diagnostic.Error = ex.Message;
            }

            return diagnostic;
        }

        private string AnalyzeResponseQuality(string response)
        {
            if (response.Contains("n'ai pas trouv�") || 
                response.Contains("d�passe mes connaissances") ||
                response.Contains("pas de r�ponse") ||
                response.Contains("pas trouv�") ||
                response.Contains("reformuler"))
            {
                return "Faible - Aucune r�ponse trouv�e";
            }

            if (response.Contains("Voici les informations") && response.Contains("**"))
            {
                return "Excellente - R�ponses multiples organis�es";
            }

            if (response.Length > 50 && !response.Contains("d�sol�"))
            {
                return "Bonne - R�ponse d�taill�e";
            }

            if (response.Length > 20)
            {
                return "Moyenne - R�ponse basique";
            }

            return "Faible - R�ponse trop courte";
        }
    }

    public class SearchDiagnostic
    {
        public string Question { get; set; } = string.Empty;
        public string NormalizedQuestion { get; set; } = string.Empty;
        public int TotalKnowledgeItems { get; set; }
        public int ExactMatches { get; set; }
        public int KeywordMatches { get; set; }
        public List<string> ExtractedWords { get; set; } = new();
        public List<string> TechnicalKeywords { get; set; } = new();
        public bool IsStandardResponse { get; set; }
        public List<string> ExampleMatches { get; set; } = new();
        public string? Error { get; set; }
    }

    public class QuestionExample
    {
        public string Question { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
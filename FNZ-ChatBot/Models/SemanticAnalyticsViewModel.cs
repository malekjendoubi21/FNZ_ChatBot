namespace FNZ_ChatBot.Models
{
    public class SemanticAnalyticsViewModel
    {
        public List<QuestionFrequency> MostFrequentQuestions { get; set; } = new();
        public List<QuestionFrequency> UnansweredQuestions { get; set; } = new();
        public int TotalConversations { get; set; }
        public int ActiveKnowledgeItems { get; set; }
        public double SuccessRate { get; set; }
    }

    public class QuestionFrequency
    {
        public string Question { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FNZ_ChatBot.Services
{
    public class SemanticSearch
    {
        private readonly MLContext _mlContext = new();
        private readonly ITransformer _model;
        private readonly DataViewSchema _schema;
        private readonly List<QuestionEntry> _knowledgeBase;

        public SemanticSearch(List<QuestionEntry> knowledgeBase)
        {
            _knowledgeBase = knowledgeBase;

            // 1. Entraîner le modèle une seule fois sur toutes les questions
            var data = _mlContext.Data.LoadFromEnumerable(
                _knowledgeBase.Select(q => new { Text = q.Question })
            );
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Text");
            _model = pipeline.Fit(data);
            _schema = data.Schema;

            // 2. Calculer les embeddings pour chaque question
            foreach (var entry in _knowledgeBase)
            {
                entry.Embedding = GetEmbedding(entry.Question);
            }
        }

        private float[] GetEmbedding(string text)
        {
            var singleData = _mlContext.Data.LoadFromEnumerable(new[] { new { Text = text } });
            var transformed = _model.Transform(singleData);
            return _mlContext.Data.CreateEnumerable<EmbeddingVector>(transformed, reuseRowObject: false).First().Features;
        }

        public List<QuestionEntry> Search(string query, int topN = 3, float threshold = 0.5f)
        {
            var queryVector = GetEmbedding(query);
            return _knowledgeBase
                .Select(q => new { q, score = CosineSimilarity(queryVector, q.Embedding) })
                .Where(x => x.score >= threshold) // <-- filtrage par seuil
                .OrderByDescending(x => x.score)
                .Take(topN)
                .Select(x => x.q)
                .ToList();
        }


        private float CosineSimilarity(float[] v1, float[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
                throw new ArgumentException("Les vecteurs doivent avoir la même taille et ne pas être null.");

            float dot = 0f, norm1 = 0f, norm2 = 0f;
            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                norm1 += v1[i] * v1[i];
                norm2 += v2[i] * v2[i];
            }
            return dot / (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        private class EmbeddingVector
        {
            public float[] Features { get; set; }
        }
    }

    public class QuestionEntry
    {
        public string Question { get; set; }
        public string Response { get; set; }
        public float[] Embedding { get; set; }
    }
}

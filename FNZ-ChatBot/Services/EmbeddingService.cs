using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text;

public class EmbeddingService
{
    private readonly InferenceSession _session;

    public EmbeddingService(string modelPath)
    {
        _session = new InferenceSession(modelPath);
    }

    public float[] GetEmbedding(string text)
    {
        // Prétraitement : convertir en tokens (ici simplifié, normalement il faut un tokenizer BERT)
        var tokens = Encoding.UTF8.GetBytes(text.ToLower()).Select(b => (float)b).ToArray();

        // Créer le tenseur d’entrée (forme simplifiée : batch 1 x taille)
        var input = new DenseTensor<float>(new[] { 1, tokens.Length });
        for (int i = 0; i < tokens.Length; i++) input[0, i] = tokens[i];

        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", input) };

        using var results = _session.Run(inputs);
        var embedding = results.First().AsEnumerable<float>().ToArray();

        return embedding;
    }

    public static float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return dot / (float)(Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}

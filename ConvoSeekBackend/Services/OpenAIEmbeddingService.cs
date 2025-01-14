using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;

namespace ConvoSeekBackend.Services
{
    public class OpenAIEmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _client;
        private readonly string _apiKey;
        private readonly string _embeddingModel;

        public OpenAIEmbeddingService(IOptions<OpenAIOptions> options)
        {
            _apiKey = options.Value.ApiKey;
            _embeddingModel = options.Value.EmbeddingModel;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentNullException(nameof(_apiKey), "OpenAI API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_embeddingModel))
            {
                throw new ArgumentNullException(nameof(_apiKey), "Embedding model is not configured.");
            }

            _client = new EmbeddingClient(_embeddingModel, _apiKey);        
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbedding(string inputText)
        {
            ClientResult<OpenAIEmbedding> embeddings = await _client.GenerateEmbeddingAsync(inputText);

            return embeddings.Value.ToFloats();
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace ConvoSeekBackend.Services
{
    public class OpenAIEmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _client;
        private readonly string _apiKey;
        private readonly string _embeddingModel;
        private readonly ILogger<OpenAIEmbeddingService> _logger;

        public OpenAIEmbeddingService(IOptions<OpenAIOptions> options, ILogger<OpenAIEmbeddingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = options.Value.ApiKey;
            _embeddingModel = options.Value.EmbeddingModel;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentNullException(nameof(_apiKey), "OpenAI API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_embeddingModel))
            {
                throw new ArgumentNullException(nameof(_embeddingModel), "Embedding model is not configured.");
            }

            _client = new EmbeddingClient(_embeddingModel, _apiKey);
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbedding(string inputText)
        {
            try
            {
                var embeddings = await _client.GenerateEmbeddingAsync(inputText);

                if (embeddings == null || embeddings.Value == null)
                {
                    _logger.LogWarning("Embedding generation returned null for input: {InputText}", inputText);
                    throw new InvalidOperationException("Failed to generate embedding.");
                }

                return embeddings.Value.ToFloats();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while generating embedding for input: {InputText}", inputText);
                throw new Exception("A network error occurred while generating the embedding. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating embedding for input: {InputText}", inputText);
                throw; // Re-throw to allow higher layers to handle it.
            }
        }
    }
}

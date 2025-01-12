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
        private readonly string _model;

        public OpenAIEmbeddingService(IOptions<OpenAIOptions> options)
        {
            _apiKey = options.Value.ApiKey;
            _model = options.Value.EmbeddingModel;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentNullException(nameof(_apiKey), "OpenAI API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_model))
            {
                throw new ArgumentNullException(nameof(_apiKey), "Embedding model is not configured.");
            }

            _client = new EmbeddingClient(_model, _apiKey);        
        }

        public async Task GenerateEmbeddings()
        {
            ClientResult<OpenAIEmbedding> returnValue = await _client.GenerateEmbeddingAsync("Hello World!");

            var floats = returnValue.Value.ToFloats();

            throw new NotImplementedException();
        }

        /*
         * public class EmbeddingGenerator(Uri endpoint, AzureKeyCredential credentials, string deploymentName)
{
    // This is for people using OpenAI directly like me.
    // If you are using OpenAI with Azure, the 2 params endpoint and credentials
    // will be what you need
    private readonly OpenAIClient OpenAiClient = new("YOUR_OPENAI_API_KEY");

    public async Task<Vector> GenerateEmbeddingAsync(string text)
    {
        var embeddingOptions = new EmbeddingsOptions
        {
            // deploymentName is the model you want to use.
            // e.g: text-embedding-ada-002
            DeploymentName = deploymentName,
            Input = { text }
        };

        var response = await OpenAiClient.GetEmbeddingsAsync(embeddingOptions);

        if (response.Value.Data.Count > 0)
            return new Vector(response.Value.Data[0].Embedding.ToArray());
        throw new Exception("Failed to generate embedding.");
    }
} 
         * 
         */
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI;

namespace ConvoSeekBackend.Services
{
    public class OpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<OpenAIChatService> _logger;

        public OpenAIChatService(IOptions<OpenAIOptions> options, ILogger<OpenAIChatService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = options.Value.ApiKey;
            _model = options.Value.Model;

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentNullException(nameof(_apiKey), "OpenAI API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_model))
            {
                throw new ArgumentNullException(nameof(_model), "OpenAI model is not configured.");
            }

            _openAIClient = new OpenAIClient(_apiKey);
            _chatClient = _openAIClient.GetChatClient(_model);
        }

        public async Task<string> AnswerQuestion(string question, List<string> context)
        {
            try
            {
                var prompt = $$"""
                    Please answer the question using only the provided context.

                    Question: {{question}}

                    Context:
                    {{string.Join("\n", context)}}
                    """;

                var result = string.Empty;

                var completionUpdates = _chatClient.CompleteChatStreamingAsync(prompt);

                await foreach (var completionUpdate in completionUpdates)
                {
                    foreach (var contentPart in completionUpdate.ContentUpdate)
                    {
                        result += contentPart.Text;
                    }
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    _logger.LogWarning("Chat completion returned empty result for question: {Question}", question);
                    throw new InvalidOperationException("Failed to generate a response.");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while generating chat completion for question: {Question}", question);
                throw new Exception("A network error occurred while generating the response. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating chat completion for question: {Question}", question);
                throw; // Re-throw to allow higher layers to handle it.
            }
        }
    }
}
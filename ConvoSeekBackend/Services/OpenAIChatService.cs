using OpenAI.Chat;
using OpenAI;
using Microsoft.Extensions.Options;
using System.ClientModel;

namespace ConvoSeekBackend.Services
{
    public class OpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        private readonly string _apiKey;
        private readonly string _model;


        public OpenAIChatService(IOptions<OpenAIOptions> options)
        {
            _apiKey = options.Value.ApiKey;
            _model = options.Value.Model;

            if (string.IsNullOrWhiteSpace(_apiKey)) throw new ArgumentNullException(nameof(_apiKey), "OpenAI API key is not configured.");
            if (string.IsNullOrWhiteSpace(_model)) throw new ArgumentNullException(nameof(_model), "OpenAI model is not configured.");

            _openAIClient = new OpenAIClient(_apiKey);
            _chatClient = _openAIClient.GetChatClient(_model);
        }

        public async Task<string> AnswerQuestion(string question, List<string> context)
        {
            var rvl = string.Empty;

            var prompt = $$"""
                Please answer the question using only the provided context.

                Question: {{question}}

                Context:
                {{string.Join("\n", context)}}
                """;

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = _chatClient.CompleteChatStreamingAsync(prompt);

            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
                {
                    rvl += contentPart.Text;
                }
            }
            return rvl;
        }
    }
}

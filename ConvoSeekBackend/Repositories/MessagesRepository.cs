using ConvoSeekBackend.Data;
using ConvoSeekBackend.Helpers;
using ConvoSeekBackend.Models;
using ConvoSeekBackend.Services;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace ConvoSeekBackend.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ConvoSeekBackendContext _context;
        private readonly IEmbeddingService _embeddingService;
        private readonly IChatService _chatService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessagesRepository> _logger;

        public MessagesRepository(
            ConvoSeekBackendContext context,
            IEmbeddingService embeddingService,
            IChatService chatService,
            IConfiguration configuration,
            ILogger<MessagesRepository> logger)
        {
            _context = context;
            _embeddingService = embeddingService;
            _chatService = chatService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> SearchAsync(string q, User user)
        {
            try
            {
                var queryEmbeddingArray = await _embeddingService.GenerateEmbedding(q);
                var queryEmbedding = new Pgvector.Vector(queryEmbeddingArray);

                var results = await _context.Messages
                    .Where(m => m.UserId == user.Id)
                    .OrderBy(m => m.Embedding.L2Distance(queryEmbedding))
                    .Take(10)
                    .ToListAsync();

                var encryptionKey = _configuration["Encryption:Key"];
                var encryptionHelper = new EncryptionHelper(encryptionKey!);

                var decryptedResults = results
                    .Select(m => encryptionHelper.Decrypt(m.EncryptedText))
                    .ToList();

                var rvl = await _chatService.AnswerQuestion(q, decryptedResults);
                return rvl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SearchAsync: {ex.Message}");

                // Rethrow or encapsulate the exception
                throw new InvalidOperationException("An error occurred while processing the search request.", ex);
            }
        }
    }
}
using ConvoSeekBackend.Data;
using ConvoSeekBackend.Helpers;
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

        public MessagesRepository(
            ConvoSeekBackendContext context,
            IEmbeddingService embeddingService,
            IChatService chatService,
            IConfiguration configuration)
        {
            _context = context;
            _embeddingService = embeddingService;
            _chatService = chatService;
            _configuration = configuration;
        }

        public async Task<string> SearchAsync(string q)
        {
            var queryEmbeddingArray = await _embeddingService.GenerateEmbedding(q);
            var queryEmbedding = new Pgvector.Vector(queryEmbeddingArray);

            var results = await _context.Messages
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
    }
}
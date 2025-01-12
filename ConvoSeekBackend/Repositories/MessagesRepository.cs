using ConvoSeekBackend.Data;
using ConvoSeekBackend.Models;
using ConvoSeekBackend.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ConvoSeekBackend.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ConvoSeekBackendContext _context;

        public MessagesRepository(ConvoSeekBackendContext context)
        {
            _context = context;
        }

        public async Task<List<string>> SearchAsync(string q)
        {
            var results = await _context.Messages
                .Where(m => EF.Functions.ILike(m.Text, $"%{q}%"))
                .ToListAsync();

            // return results;

            throw new NotImplementedException();
        }


        /*
         * 
         * public async Task<List<Post>> SearchAsync(string searchText)
    {
        // this threshold is kinda like the accuracy of the search.
        // I would recommend to start with 0.5, which shoud
        // give you something. Normally 0.5 gives you too
        // much info. Tweaking it to the sweet spot is a fun experience.
        // it is different for any data.
        const double threshold = 0.50;

        // Generate embedding for the search text using OpenAI
        var queryEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(searchText);

        // find neighbors in vector space and only take 5.
        // it also orders based on title embedding to show relevance of the order
        var posts = await db.Posts
            .Where(post => post.TitleEmbedding!.L2Distance(queryEmbedding) < threshold ||
                           post.ContentEmbedding!.L2Distance(queryEmbedding) < threshold)
            .OrderBy(post => post.TitleEmbedding!.L2Distance(queryEmbedding) < threshold)
            .Take(5)
            .ToListAsync();

        return posts;
    } 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
    }
}
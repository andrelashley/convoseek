using ConvoSeekBackend.Models;

namespace ConvoSeekBackend.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<string>> SearchAsync(string q);
    }
}

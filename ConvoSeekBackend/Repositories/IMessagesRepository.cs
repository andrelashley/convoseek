using ConvoSeekBackend.Models;

namespace ConvoSeekBackend.Repositories
{
    public interface IMessagesRepository
    {
        Task<string> SearchAsync(string q);
    }
}

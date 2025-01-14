namespace ConvoSeekBackend.Services
{
    public interface IChatService
    {
        Task<string> AnswerQuestion(string question, List<string> context);
    }
}

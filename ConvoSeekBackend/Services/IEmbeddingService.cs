namespace ConvoSeekBackend.Services
{
    public interface IEmbeddingService
    {
        Task<ReadOnlyMemory<float>> GenerateEmbedding(string inputText);
    }
}

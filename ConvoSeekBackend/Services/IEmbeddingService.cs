namespace ConvoSeekBackend.Services
{
    public interface IEmbeddingService
    {
        Task<ReadOnlyMemory<float>> GenerateEmbedding(string inputText);
        Task<List<ReadOnlyMemory<float>>> GenerateEmbeddings(List<string> inputText);
    }
}

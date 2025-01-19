namespace ConvoSeekBackend.Dtos
{
    public class MessageDto
    {
        public string? Author { get; set; }
        public string? Message { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}

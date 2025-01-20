namespace ConvoSeekBackend.ViewModels
{
    public class MessageViewModel
    {
        public string? Sender { get; set; }
        public string? Text { get; set; }
        public DateTimeOffset? Date { get; set; }
    }
}

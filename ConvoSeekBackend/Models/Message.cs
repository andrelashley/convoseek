using Pgvector;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvoSeekBackend.Models
{
    public class Message
    {
        public Guid MessageId { get; set; }

        [Required]
        public string Text { get; set; }

        [Column(TypeName = "vector(1536)")]
        public Vector? Embedding { get; set; }
    }
}

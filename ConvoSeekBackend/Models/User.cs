using Microsoft.AspNetCore.Identity;
using System;

namespace ConvoSeekBackend.Models
{
    public class User : IdentityUser
    {
        public string Handle { get; set; } = string.Empty;
        public bool IsSubscriptionActive { get; set; }
        public string? SubscriptionId { get; set; }
        public string? CustomerId { get; set; }
        public DateTimeOffset? CustomerCreatedAt { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;

namespace ConvoSeekBackend.Models
{
    public class User : IdentityUser
    {
        public string Handle { get; set; } = string.Empty;
    }
}

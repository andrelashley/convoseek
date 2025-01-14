using Microsoft.EntityFrameworkCore;
using ConvoSeekBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ConvoSeekBackend.Data
{
    public class ConvoSeekBackendContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;

        public ConvoSeekBackendContext (DbContextOptions<ConvoSeekBackendContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Message> Messages { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("vector");
                      
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("ConvoSeekBackendContext")
                    ?? throw new InvalidOperationException("Connection string 'ConvoSeekBackendContext' not found.");

                optionsBuilder.UseNpgsql(connectionString, o => o.UseVector());
            }
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AfroEvent.Models;

namespace AfroEvent.Data
{
    /// <summary>
    /// Context principal d'Entity Framework Core pour AfroEvent.
    /// Hérite de IdentityDbContext pour intégrer ASP.NET Core Identity.
    /// </summary>
    public class AfroEventDbContext : IdentityDbContext
    {
        public AfroEventDbContext(DbContextOptions<AfroEventDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<EventEntity> Events { get; set; } = null!;
        public DbSet<SpeakerEntity> Speakers { get; set; } = null!;
        public DbSet<AgendaItemEntity> AgendaItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed initial categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Hackathon" },
                new Category { Id = 2, Name = "Bootcamp" },
                new Category { Id = 3, Name = "Conférence" },
                new Category { Id = 4, Name = "Workshop" },
                new Category { Id = 5, Name = "Concert" }
            );

            // Configure Price precision
            builder.Entity<EventEntity>()
                .Property(e => e.Price)
                .HasPrecision(18, 2);
        }
    }
}

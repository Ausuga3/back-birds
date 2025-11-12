using Microsoft.EntityFrameworkCore;
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;

namespace BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Persistence
{
    public class BirdsDbContext : DbContext
    {
        public BirdsDbContext(DbContextOptions<BirdsDbContext> options) : base(options) { }

        public DbSet<BirdEntity> Birds { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BirdEntity>(eb =>
            {
                eb.ToTable("Birds");
                
                eb.HasKey(b => b.Id);
                
                eb.Property(b => b.CommonName)
                    .IsRequired()
                    .HasMaxLength(200);
                
                eb.Property(b => b.ScientificName)
                    .IsRequired()
                    .HasMaxLength(200);
                
                eb.Property(b => b.Family)
                    .IsRequired()
                    .HasConversion<string>(); // Almacenar enum como string
                
                eb.Property(b => b.ConservationStatus)
                    .IsRequired()
                    .HasConversion<string>(); // Almacenar enum como string
                
                eb.Property(b => b.Notes)
                    .IsRequired()
                    .HasMaxLength(1000);
                
                eb.Property(b => b.Created_At)
                    .IsRequired();
                
                eb.Property(b => b.Updated_At)
                    .IsRequired();
                
                eb.Property(b => b.Created_By)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

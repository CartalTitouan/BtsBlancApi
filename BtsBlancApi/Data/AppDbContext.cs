using BtsBlancApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BtsBlancApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Evenement> Evenements => Set<Evenement>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inscription>()
            .HasIndex(i => new { i.UserId, i.EvenementId })
            .IsUnique();
    }
}

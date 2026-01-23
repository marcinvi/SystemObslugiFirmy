using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Klient> Klienci { get; set; }
    public DbSet<Produkt> Produkty { get; set; }
    public DbSet<Zgloszenie> Zgloszenia { get; set; }
    public DbSet<Dzialanie> Dzialania { get; set; }
    public DbSet<Plik> Pliki { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Zgloszenie>()
            .HasOne(z => z.Klient)
            .WithMany(k => k.Zgloszenia)
            .HasForeignKey(z => z.IdKlienta)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Zgloszenie>()
            .HasOne(z => z.Produkt)
            .WithMany(p => p.Zgloszenia)
            .HasForeignKey(z => z.IdProduktu)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Zgloszenie>()
            .HasOne(z => z.AssignedUser)
            .WithMany(u => u.AssignedZgloszenia)
            .HasForeignKey(z => z.PrzypisanyDo)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Dzialanie>()
            .HasOne(d => d.Zgloszenie)
            .WithMany(z => z.Dzialania)
            .HasForeignKey(d => d.IdZgloszenia)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Plik>()
            .HasOne(p => p.Zgloszenie)
            .WithMany(z => z.Pliki)
            .HasForeignKey(p => p.IdZgloszenia)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<Zgloszenie>()
            .HasIndex(z => z.NrZgloszenia)
            .IsUnique();

        modelBuilder.Entity<Zgloszenie>()
            .HasIndex(z => z.StatusOgolny);

        modelBuilder.Entity<Zgloszenie>()
            .HasIndex(z => z.DataZgloszenia);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.IsActive)
            .HasConversion<int>();

        modelBuilder.Entity<Klient>()
            .HasIndex(k => k.Telefon);

        modelBuilder.Entity<Klient>()
            .HasIndex(k => k.Email);
    }
}

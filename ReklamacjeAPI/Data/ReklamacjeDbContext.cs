using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Data;

public class ReklamacjeDbContext : DbContext
{
    public ReklamacjeDbContext(DbContextOptions<ReklamacjeDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Klient> Klienci { get; set; }
    public DbSet<Produkt> Produkty { get; set; }
    public DbSet<Zgloszenie> Zgloszenia { get; set; }
    public DbSet<Dzialanie> Dzialania { get; set; }
    public DbSet<Plik> Pliki { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Uzytkownicy");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.DisplayName);
            entity.Property(e => e.Email);
            entity.Property(e => e.Role);
            entity.Property(e => e.IsActive).HasConversion<int>();
        });

        // Klient
        modelBuilder.Entity<Klient>(entity =>
        {
            entity.ToTable("Klienci");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImieNazwisko).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(20);
        });

        // Produkt
        modelBuilder.Entity<Produkt>(entity =>
        {
            entity.ToTable("Produkty");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nazwa).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Producent).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
        });

        // Zgloszenie
        modelBuilder.Entity<Zgloszenie>(entity =>
        {
            entity.ToTable("Zgloszenia");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NrZgloszenia).IsRequired().HasMaxLength(50);

            // Relationships
            entity.HasOne(e => e.Klient)
                .WithMany(k => k.Zgloszenia)
                .HasForeignKey(e => e.IdKlienta)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Produkt)
                .WithMany(p => p.Zgloszenia)
                .HasForeignKey(e => e.IdProduktu)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.AssignedUser)
                .WithMany(u => u.AssignedZgloszenia)
                .HasForeignKey(e => e.PrzypisanyDo)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Dzialanie
        modelBuilder.Entity<Dzialanie>(entity =>
        {
            entity.ToTable("Dzialania");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Zgloszenie)
                .WithMany(z => z.Dzialania)
                .HasForeignKey(e => e.IdZgloszenia)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.IdUzytkownika)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Plik
        modelBuilder.Entity<Plik>(entity =>
        {
            entity.ToTable("Pliki");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NazwaPliku).IsRequired().HasMaxLength(255);

            entity.HasOne(e => e.Zgloszenie)
                .WithMany(z => z.Pliki)
                .HasForeignKey(e => e.IdZgloszenia)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

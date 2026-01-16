using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Data;

public class ReklamacjeDbContext : DbContext
{
    public ReklamacjeDbContext(DbContextOptions<ReklamacjeDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Uzytkownik> Uzytkownicy { get; set; }
    public DbSet<Klient> Klienci { get; set; }
    public DbSet<Produkt> Produkty { get; set; }
    public DbSet<Zgloszenie> Zgloszenia { get; set; }
    public DbSet<Dzialanie> Dzialania { get; set; }
    public DbSet<Plik> Pliki { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Uzytkownik
        modelBuilder.Entity<Uzytkownik>(entity =>
        {
            entity.ToTable("Uzytkownicy");
            entity.HasKey(e => e.IdUzytkownika);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Haslo).IsRequired();
            entity.Property(e => e.NazwaWyswietlana).IsRequired().HasMaxLength(100);
        });

        // Klient
        modelBuilder.Entity<Klient>(entity =>
        {
            entity.ToTable("Klienci");
            entity.HasKey(e => e.IdKlienta);
            entity.Property(e => e.ImieNazwisko).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(20);
        });

        // Produkt
        modelBuilder.Entity<Produkt>(entity =>
        {
            entity.ToTable("Produkty");
            entity.HasKey(e => e.IdProduktu);
            entity.Property(e => e.Nazwa).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Producent).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
        });

        // Zgloszenie
        modelBuilder.Entity<Zgloszenie>(entity =>
        {
            entity.ToTable("Zgloszenia");
            entity.HasKey(e => e.IdZgloszenia);
            entity.Property(e => e.NrZgloszenia).IsRequired().HasMaxLength(50);

            // Relationships
            entity.HasOne(e => e.Klient)
                .WithMany(k => k.Zgloszenia)
                .HasForeignKey(e => e.IdKlienta)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Produkt)
                .WithMany(p => p.Zgloszenia)
                .HasForeignKey(e => e.IdProduktu)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UzytkownikStworzyl)
                .WithMany(u => u.ZgloszeniaStworzone)
                .HasForeignKey(e => e.IdUzytkownikaStworzyl)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UzytkownikPrzypisany)
                .WithMany(u => u.ZgloszeniaPrzypisane)
                .HasForeignKey(e => e.IdUzytkownikaPrzypisany)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Dzialanie
        modelBuilder.Entity<Dzialanie>(entity =>
        {
            entity.ToTable("Dzialania");
            entity.HasKey(e => e.IdDzialania);

            entity.HasOne(e => e.Zgloszenie)
                .WithMany(z => z.Dzialania)
                .HasForeignKey(e => e.IdZgloszenia)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Uzytkownik)
                .WithMany(u => u.Dzialania)
                .HasForeignKey(e => e.IdUzytkownika)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Plik
        modelBuilder.Entity<Plik>(entity =>
        {
            entity.ToTable("Pliki");
            entity.HasKey(e => e.IdPliku);
            entity.Property(e => e.NazwaPliku).IsRequired().HasMaxLength(255);

            entity.HasOne(e => e.Zgloszenie)
                .WithMany(z => z.Pliki)
                .HasForeignKey(e => e.IdZgloszenia)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoguelikeDbConsole.Models;

namespace RoguelikeDbConsole.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string? _connectionString;

        // Конструктор без параметров (используется в Program.cs при new ApplicationDbContext())
        public ApplicationDbContext()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not found in appsettings.json");
        }

        // Конструктор с параметрами (используется в DI)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Enemy> Enemies { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<RunItem> RunItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Если опции не сконфигурированы и есть строка подключения - используем её
            if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseNpgsql(_connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальный индекс на название предмета
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Name)
                .IsUnique()
                .HasDatabaseName("idx_unique_item_name");

            // Уникальная пара (run_id, item_id)
            modelBuilder.Entity<RunItem>()
                .HasIndex(ri => new { ri.RunId, ri.ItemId })
                .IsUnique()
                .HasDatabaseName("unique_run_item");

            // Отношения
            modelBuilder.Entity<Run>()
                .HasOne(r => r.Character)
                .WithMany(c => c.Runs)
                .HasForeignKey(r => r.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RunItem>()
                .HasOne(ri => ri.Run)
                .WithMany(r => r.RunItems)
                .HasForeignKey(ri => ri.RunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RunItem>()
                .HasOne(ri => ri.Item)
                .WithMany(i => i.RunItems)
                .HasForeignKey(ri => ri.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка типов для PostgreSQL (UUID генерация)
            modelBuilder.Entity<Character>()
                .Property(c => c.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Item>()
                .Property(i => i.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Enemy>()
                .Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Run>()
                .Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<RunItem>()
                .Property(ri => ri.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Настройка Decimal точности
            modelBuilder.Entity<Character>()
                .Property(c => c.Health)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Character>()
                .Property(c => c.Damage)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Character>()
                .Property(c => c.Speed)
                .HasPrecision(4, 2);

            modelBuilder.Entity<Item>()
                .Property(i => i.DamageBonus)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Item>()
                .Property(i => i.HealthBonus)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Item>()
                .Property(i => i.SpeedBonus)
                .HasPrecision(4, 2);

            modelBuilder.Entity<Run>()
                .Property(r => r.Score)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Enemy>()
                .Property(e => e.Health)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Enemy>()
                .Property(e => e.Damage)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Enemy>()
                .Property(e => e.Speed)
                .HasPrecision(4, 2);
        }
    }
}
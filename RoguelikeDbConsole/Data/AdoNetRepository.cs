using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RoguelikeDbConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace RoguelikeDbConsole.Data
{
    public class AdoNetRepository : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;
        private bool _disposed = false;

        public AdoNetRepository()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private NpgsqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        // ============= CRUD для Characters =============
        public async Task<IEnumerable<Character>> GetAllCharactersAsync()
        {
            const string sql = "SELECT * FROM characters ORDER BY name";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Character>(sql);
        }

        public async Task<Character?> GetCharacterByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM characters WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Character>(sql, new { Id = id });
        }

        public async Task<Guid> CreateCharacterAsync(Character character)
        {
            const string sql = @"
                INSERT INTO characters (id, name, health, damage, speed, is_unlocked, created_at, updated_at)
                VALUES (@Id, @Name, @Health, @Damage, @Speed, @IsUnlocked, @CreatedAt, @UpdatedAt)
                RETURNING id";

            character.Id = Guid.NewGuid();
            character.CreatedAt = DateTime.UtcNow;
            character.UpdatedAt = DateTime.UtcNow;

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<Guid>(sql, character);
        }

        public async Task<bool> UpdateCharacterAsync(Character character)
        {
            const string sql = @"
                UPDATE characters 
                SET name = @Name, 
                    health = @Health, 
                    damage = @Damage, 
                    speed = @Speed, 
                    is_unlocked = @IsUnlocked,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            character.UpdatedAt = DateTime.UtcNow;

            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, character);
            return affected > 0;
        }

        public async Task<bool> DeleteCharacterAsync(Guid id)
        {
            const string sql = "DELETE FROM characters WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ============= CRUD для Items =============
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            const string sql = "SELECT * FROM items ORDER BY name";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Item>(sql);
        }

        public async Task<Item?> GetItemByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM items WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Item>(sql, new { Id = id });
        }

        public async Task<Guid> CreateItemAsync(Item item)
        {
            const string sql = @"
                INSERT INTO items (id, name, description, item_type, damage_bonus, health_bonus, speed_bonus, is_cursed, created_at)
                VALUES (@Id, @Name, @Description, @ItemType, @DamageBonus, @HealthBonus, @SpeedBonus, @IsCursed, @CreatedAt)
                RETURNING id";

            item.Id = Guid.NewGuid();
            item.CreatedAt = DateTime.UtcNow;

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<Guid>(sql, item);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            const string sql = @"
                UPDATE items 
                SET name = @Name, 
                    description = @Description,
                    item_type = @ItemType,
                    damage_bonus = @DamageBonus, 
                    health_bonus = @HealthBonus, 
                    speed_bonus = @SpeedBonus,
                    is_cursed = @IsCursed
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, item);
            return affected > 0;
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            const string sql = "DELETE FROM items WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ============= CRUD для Runs =============
        public async Task<IEnumerable<Run>> GetAllRunsAsync()
        {
            const string sql = "SELECT * FROM runs ORDER BY started_at DESC";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Run>(sql);
        }

        public async Task<Run?> GetRunByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM runs WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Run>(sql, new { Id = id });
        }

        public async Task<Guid> CreateRunAsync(Run run)
        {
            const string sql = @"
                INSERT INTO runs (id, character_id, run_seed, floor_number, score, is_completed, started_at, completed_at, total_time_seconds)
                VALUES (@Id, @CharacterId, @RunSeed, @FloorNumber, @Score, @IsCompleted, @StartedAt, @CompletedAt, @TotalTimeSeconds)
                RETURNING id";

            run.Id = Guid.NewGuid();
            run.StartedAt = DateTime.UtcNow;

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<Guid>(sql, run);
        }

        public async Task<bool> UpdateRunAsync(Run run)
        {
            const string sql = @"
                UPDATE runs 
                SET character_id = @CharacterId,
                    run_seed = @RunSeed,
                    floor_number = @FloorNumber,
                    score = @Score,
                    is_completed = @IsCompleted,
                    completed_at = @CompletedAt,
                    total_time_seconds = @TotalTimeSeconds
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, run);
            return affected > 0;
        }

        public async Task<bool> DeleteRunAsync(Guid id)
        {
            const string sql = "DELETE FROM runs WHERE id = @Id";
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ============= Сложные запросы ADO.NET =============
        public async Task<IEnumerable<dynamic>> GetCharacterStatisticsAsync()
        {
            const string sql = @"
                SELECT 
                    c.name AS CharacterName,
                    COUNT(r.id) AS RunsCount,
                    COALESCE(AVG(r.score), 0) AS AverageScore,
                    COALESCE(MAX(r.score), 0) AS MaxScore,
                    COALESCE(AVG(r.floor_number), 0) AS AverageFloor
                FROM characters c
                LEFT JOIN runs r ON c.id = r.character_id
                WHERE r.is_completed = TRUE
                GROUP BY c.id, c.name
                ORDER BY RunsCount DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync(sql);
        }

        public async Task<IEnumerable<dynamic>> GetItemsUsageStatisticsAsync()
        {
            const string sql = @"
                SELECT 
                    i.name AS ItemName,
                    i.item_type AS ItemType,
                    COUNT(ri.id) AS UsageCount,
                    COALESCE(AVG(i.damage_bonus), 0) AS AvgDamageBonus
                FROM items i
                LEFT JOIN run_items ri ON i.id = ri.item_id
                GROUP BY i.id, i.name, i.item_type
                HAVING COUNT(ri.id) > 0
                ORDER BY UsageCount DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync(sql);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connection?.Close();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
}
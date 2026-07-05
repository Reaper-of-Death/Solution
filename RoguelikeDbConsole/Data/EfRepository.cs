using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoguelikeDbConsole.Models;

namespace RoguelikeDbConsole.Data
{
    public class EfRepository
    {
        private readonly ApplicationDbContext _context;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============= CRUD для Characters =============
        public async Task<IEnumerable<Character>> GetAllCharactersAsync()
        {
            return await _context.Characters
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Character?> GetCharacterByIdAsync(Guid id)
        {
            return await _context.Characters
                .Include(c => c.Runs)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Character> CreateCharacterAsync(Character character)
        {
            character.Id = Guid.NewGuid();
            character.CreatedAt = DateTime.UtcNow;
            character.UpdatedAt = DateTime.UtcNow;

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<Character> UpdateCharacterAsync(Character character)
        {
            character.UpdatedAt = DateTime.UtcNow;
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<bool> DeleteCharacterAsync(Guid id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null) return false;

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============= CRUD для Items =============
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.RunItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            item.Id = Guid.NewGuid();
            item.CreatedAt = DateTime.UtcNow;

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============= CRUD для Runs =============
        public async Task<IEnumerable<Run>> GetAllRunsAsync()
        {
            return await _context.Runs
                .Include(r => r.Character)
                .Include(r => r.RunItems)
                .ThenInclude(ri => ri.Item)
                .OrderByDescending(r => r.StartedAt)
                .ToListAsync();
        }

        public async Task<Run?> GetRunByIdAsync(Guid id)
        {
            return await _context.Runs
                .Include(r => r.Character)
                .Include(r => r.RunItems)
                .ThenInclude(ri => ri.Item)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Run> CreateRunAsync(Run run)
        {
            run.Id = Guid.NewGuid();
            run.StartedAt = DateTime.UtcNow;

            await _context.Runs.AddAsync(run);
            await _context.SaveChangesAsync();
            return run;
        }

        public async Task<Run> UpdateRunAsync(Run run)
        {
            _context.Runs.Update(run);
            await _context.SaveChangesAsync();
            return run;
        }

        public async Task<bool> DeleteRunAsync(Guid id)
        {
            var run = await _context.Runs.FindAsync(id);
            if (run == null) return false;

            _context.Runs.Remove(run);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============= CRUD для RunItems =============
        public async Task<RunItem> AddItemToRunAsync(Guid runId, Guid itemId, int? pickupOrder = null)
        {
            var runItem = new RunItem
            {
                Id = Guid.NewGuid(),
                RunId = runId,
                ItemId = itemId,
                PickupOrder = pickupOrder,
                PickedUpAt = DateTime.UtcNow,
                IsUsed = false
            };

            await _context.RunItems.AddAsync(runItem);
            await _context.SaveChangesAsync();
            return runItem;
        }

        public async Task<bool> RemoveItemFromRunAsync(Guid runId, Guid itemId)
        {
            var runItem = await _context.RunItems
                .FirstOrDefaultAsync(ri => ri.RunId == runId && ri.ItemId == itemId);

            if (runItem == null) return false;

            _context.RunItems.Remove(runItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============= Сложные запросы EF с обработкой NULL =============
        public async Task<IEnumerable<CharacterStats>> GetCharacterStatsAsync()
        {
            try
            {
                // Получаем всех персонажей и считаем статистику в памяти
                var characters = await _context.Characters
                    .Include(c => c.Runs.Where(r => r.IsCompleted))
                    .ToListAsync();

                var result = characters.Select(c => new CharacterStats
                {
                    CharacterName = c.Name,
                    RunsCount = c.Runs.Count(r => r.IsCompleted),
                    AverageScore = c.Runs.Any(r => r.IsCompleted) 
                        ? c.Runs.Where(r => r.IsCompleted).Average(r => r.Score) 
                        : 0,
                    MaxScore = c.Runs.Any(r => r.IsCompleted) 
                        ? c.Runs.Where(r => r.IsCompleted).Max(r => r.Score) 
                        : 0,
                    AverageFloor = c.Runs.Any(r => r.IsCompleted) 
                        ? c.Runs.Where(r => r.IsCompleted).Average(r => r.FloorNumber) 
                        : 0
                })
                .OrderByDescending(s => s.RunsCount)
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetCharacterStatsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ItemUsageStats>> GetItemUsageStatsAsync()
        {
            try
            {
                var items = await _context.Items
                    .Include(i => i.RunItems)
                    .ToListAsync();

                var result = items
                    .Where(i => i.RunItems.Any())
                    .Select(i => new ItemUsageStats
                    {
                        ItemName = i.Name,
                        ItemType = i.ItemType,
                        UsageCount = i.RunItems.Count,
                        AvgDamageBonus = i.DamageBonus
                    })
                    .OrderByDescending(s => s.UsageCount)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetItemUsageStatsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<RunDetail>> GetRunDetailsAsync()
        {
            try
            {
                var runs = await _context.Runs
                    .Include(r => r.Character)
                    .Include(r => r.RunItems)
                    .ThenInclude(ri => ri.Item)
                    .Where(r => r.IsCompleted)
                    .ToListAsync();

                var result = runs.Select(r => new RunDetail
                {
                    RunId = r.Id,
                    CharacterName = r.Character?.Name ?? "Unknown",
                    RunSeed = r.RunSeed,
                    Score = r.Score,
                    FloorNumber = r.FloorNumber,
                    TotalTimeMinutes = r.TotalTimeSeconds / 60,
                    ItemsCount = r.RunItems.Count,
                    TotalDamageBonus = r.RunItems.Sum(ri => ri.Item?.DamageBonus ?? 0)
                })
                .OrderByDescending(r => r.Score)
                .Take(10)
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetRunDetailsAsync: {ex.Message}");
                throw;
            }
        }

        // ============= DTO классы для статистики =============
        public class CharacterStats
        {
            public string CharacterName { get; set; } = string.Empty;
            public int RunsCount { get; set; }
            public decimal AverageScore { get; set; }
            public decimal MaxScore { get; set; }
            public double AverageFloor { get; set; }
        }

        public class ItemUsageStats
        {
            public string ItemName { get; set; } = string.Empty;
            public string? ItemType { get; set; }
            public int UsageCount { get; set; }
            public decimal AvgDamageBonus { get; set; }
        }

        public class RunDetail
        {
            public Guid RunId { get; set; }
            public string CharacterName { get; set; } = string.Empty;
            public string? RunSeed { get; set; }
            public decimal Score { get; set; }
            public int FloorNumber { get; set; }
            public int TotalTimeMinutes { get; set; }
            public int ItemsCount { get; set; }
            public decimal TotalDamageBonus { get; set; }
        }
    }
}
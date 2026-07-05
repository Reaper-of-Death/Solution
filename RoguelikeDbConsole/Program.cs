using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoguelikeDbConsole.Data;
using RoguelikeDbConsole.Models;
using System.IO;

namespace RoguelikeDbConsole
{
    class Program
    {
        private static IServiceProvider? _serviceProvider;
        private static AdoNetRepository? _adoRepo;
        private static EfRepository? _efRepo;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Roguelike Database Manager";

            // Инициализация DI
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            _adoRepo = new AdoNetRepository();
            
            // Создаем EfRepository через DI
            using var scope = _serviceProvider.CreateScope();
            _efRepo = scope.ServiceProvider.GetRequiredService<EfRepository>();

            await MainMenu();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Добавляем конфигурацию
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Регистрируем DbContext с правильной строкой подключения
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null)));

            services.AddScoped<EfRepository>();
        }

        private static async Task MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════╗");
                Console.WriteLine("║     ROGUELIKE DATABASE MANAGER                      ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. CRUD через ADO.NET                              ║");
                Console.WriteLine("║  2. CRUD через Entity Framework                     ║");
                Console.WriteLine("║  3. Сложные запросы (статистика)                    ║");
                Console.WriteLine("║  4. Вставка тестовых данных                         ║");
                Console.WriteLine("║  5. Очистить все данные                             ║");
                Console.WriteLine("║  0. Выход                                          ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════╝");
                Console.Write("\nВыберите опцию: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AdoNetMenu();
                        break;
                    case "2":
                        await EfMenu();
                        break;
                    case "3":
                        await ComplexQueriesMenu();
                        break;
                    case "4":
                        await InsertTestData();
                        break;
                    case "5":
                        await ClearAllData();
                        break;
                    case "0":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите Enter...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static async Task AdoNetMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════╗");
                Console.WriteLine("║     ADO.NET CRUD OPERATIONS                         ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. Список персонажей                               ║");
                Console.WriteLine("║  2. Добавить персонажа                              ║");
                Console.WriteLine("║  3. Обновить персонажа                              ║");
                Console.WriteLine("║  4. Удалить персонажа                               ║");
                Console.WriteLine("║  5. Список предметов                                ║");
                Console.WriteLine("║  6. Добавить предмет                                ║");
                Console.WriteLine("║  7. Список забегов                                  ║");
                Console.WriteLine("║  0. Назад                                           ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════╝");
                Console.Write("\nВыберите опцию: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ListCharactersAdo();
                            break;
                        case "2":
                            await AddCharacterAdo();
                            break;
                        case "3":
                            await UpdateCharacterAdo();
                            break;
                        case "4":
                            await DeleteCharacterAdo();
                            break;
                        case "5":
                            await ListItemsAdo();
                            break;
                        case "6":
                            await AddItemAdo();
                            break;
                        case "7":
                            await ListRunsAdo();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный выбор");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        private static async Task EfMenu()
        {
            // Обновляем EfRepository для каждого использования
            using var scope = _serviceProvider!.CreateScope();
            _efRepo = scope.ServiceProvider.GetRequiredService<EfRepository>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════╗");
                Console.WriteLine("║     ENTITY FRAMEWORK CRUD OPERATIONS                ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════╣");
                Console.WriteLine("║  1. Список персонажей                               ║");
                Console.WriteLine("║  2. Добавить персонажа                              ║");
                Console.WriteLine("║  3. Обновить персонажа                              ║");
                Console.WriteLine("║  4. Удалить персонажа                               ║");
                Console.WriteLine("║  5. Список предметов                                ║");
                Console.WriteLine("║  6. Добавить предмет                                ║");
                Console.WriteLine("║  7. Список забегов                                  ║");
                Console.WriteLine("║  8. Добавить забег                                  ║");
                Console.WriteLine("║  9. Добавить предмет в забег                        ║");
                Console.WriteLine("║  0. Назад                                           ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════╝");
                Console.Write("\nВыберите опцию: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ListCharactersEf();
                            break;
                        case "2":
                            await AddCharacterEf();
                            break;
                        case "3":
                            await UpdateCharacterEf();
                            break;
                        case "4":
                            await DeleteCharacterEf();
                            break;
                        case "5":
                            await ListItemsEf();
                            break;
                        case "6":
                            await AddItemEf();
                            break;
                        case "7":
                            await ListRunsEf();
                            break;
                        case "8":
                            await AddRunEf();
                            break;
                        case "9":
                            await AddItemToRunEf();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный выбор");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        // ============= ADO.NET Методы =============
        private static async Task ListCharactersAdo()
        {
            var characters = await _adoRepo!.GetAllCharactersAsync();
            Console.WriteLine("\n=== Персонажи ===");
            foreach (var c in characters)
            {
                Console.WriteLine(c);
            }
        }

        private static async Task AddCharacterAdo()
        {
            Console.Write("Имя персонажа: ");
            var name = Console.ReadLine();
            if (string.IsNullOrEmpty(name)) return;

            var character = new Character
            {
                Name = name,
                Health = 100,
                Damage = 10,
                Speed = 1.0m,
                IsUnlocked = true
            };

            var id = await _adoRepo!.CreateCharacterAsync(character);
            Console.WriteLine($"Персонаж создан с ID: {id}");
        }

        private static async Task UpdateCharacterAdo()
        {
            Console.Write("ID персонажа: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            var character = await _adoRepo!.GetCharacterByIdAsync(id);
            if (character == null)
            {
                Console.WriteLine("Персонаж не найден");
                return;
            }

            Console.WriteLine($"Текущее имя: {character.Name}");
            Console.Write("Новое имя (Enter для пропуска): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) character.Name = name;

            Console.Write($"Новое здоровье ({character.Health}): ");
            var healthInput = Console.ReadLine();
            if (decimal.TryParse(healthInput, out var health)) character.Health = health;

            await _adoRepo.UpdateCharacterAsync(character);
            Console.WriteLine("Персонаж обновлён");
        }

        private static async Task DeleteCharacterAdo()
        {
            Console.Write("ID персонажа для удаления: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            if (await _adoRepo!.DeleteCharacterAsync(id))
                Console.WriteLine("Персонаж удалён");
            else
                Console.WriteLine("Персонаж не найден");
        }

        private static async Task ListItemsAdo()
        {
            var items = await _adoRepo!.GetAllItemsAsync();
            Console.WriteLine("\n=== Предметы ===");
            foreach (var i in items)
            {
                Console.WriteLine(i);
            }
        }

        private static async Task AddItemAdo()
        {
            Console.Write("Название предмета: ");
            var name = Console.ReadLine();
            if (string.IsNullOrEmpty(name)) return;

            var item = new Item
            {
                Name = name,
                Description = "Новый предмет",
                ItemType = "passive",
                DamageBonus = 0,
                HealthBonus = 0,
                SpeedBonus = 0,
                IsCursed = false
            };

            var id = await _adoRepo!.CreateItemAsync(item);
            Console.WriteLine($"Предмет создан с ID: {id}");
        }

        private static async Task ListRunsAdo()
        {
            var runs = await _adoRepo!.GetAllRunsAsync();
            Console.WriteLine("\n=== Забеги ===");
            foreach (var r in runs)
            {
                Console.WriteLine(r);
            }
        }

        // ============= EF Методы =============
        private static async Task ListCharactersEf()
        {
            var characters = await _efRepo!.GetAllCharactersAsync();
            Console.WriteLine("\n=== Персонажи (EF) ===");
            foreach (var c in characters)
            {
                Console.WriteLine(c);
            }
        }

        private static async Task AddCharacterEf()
        {
            Console.Write("Имя персонажа: ");
            var name = Console.ReadLine();
            if (string.IsNullOrEmpty(name)) return;

            var character = new Character
            {
                Name = name,
                Health = 100,
                Damage = 10,
                Speed = 1.0m,
                IsUnlocked = true
            };

            var result = await _efRepo!.CreateCharacterAsync(character);
            Console.WriteLine($"Персонаж создан с ID: {result.Id}");
        }

        private static async Task UpdateCharacterEf()
        {
            Console.Write("ID персонажа: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            var character = await _efRepo!.GetCharacterByIdAsync(id);
            if (character == null)
            {
                Console.WriteLine("Персонаж не найден");
                return;
            }

            Console.WriteLine($"Текущее имя: {character.Name}");
            Console.Write("Новое имя (Enter для пропуска): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) character.Name = name;

            await _efRepo.UpdateCharacterAsync(character);
            Console.WriteLine("Персонаж обновлён");
        }

        private static async Task DeleteCharacterEf()
        {
            Console.Write("ID персонажа для удаления: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            if (await _efRepo!.DeleteCharacterAsync(id))
                Console.WriteLine("Персонаж удалён");
            else
                Console.WriteLine("Персонаж не найден");
        }

        private static async Task ListItemsEf()
        {
            var items = await _efRepo!.GetAllItemsAsync();
            Console.WriteLine("\n=== Предметы (EF) ===");
            foreach (var i in items)
            {
                Console.WriteLine(i);
            }
        }

        private static async Task AddItemEf()
        {
            Console.Write("Название предмета: ");
            var name = Console.ReadLine();
            if (string.IsNullOrEmpty(name)) return;

            var item = new Item
            {
                Name = name,
                Description = "Новый предмет",
                ItemType = "passive",
                DamageBonus = 0,
                HealthBonus = 0,
                SpeedBonus = 0,
                IsCursed = false
            };

            var result = await _efRepo!.CreateItemAsync(item);
            Console.WriteLine($"Предмет создан с ID: {result.Id}");
        }

        private static async Task ListRunsEf()
        {
            var runs = await _efRepo!.GetAllRunsAsync();
            Console.WriteLine("\n=== Забеги (EF) ===");
            foreach (var r in runs)
            {
                Console.WriteLine(r);
                if (r.RunItems.Any())
                {
                    Console.WriteLine("  Предметы в забеге:");
                    foreach (var ri in r.RunItems)
                    {
                        Console.WriteLine($"    - {ri.Item?.Name ?? "Unknown"} (использован: {ri.IsUsed})");
                    }
                }
            }
        }

        private static async Task AddRunEf()
        {
            var characters = await _efRepo!.GetAllCharactersAsync();
            Console.WriteLine("Доступные персонажи:");
            foreach (var c in characters)
            {
                Console.WriteLine($"  {c.Id}: {c.Name}");
            }

            Console.Write("ID персонажа: ");
            if (!Guid.TryParse(Console.ReadLine(), out var charId))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            var run = new Run
            {
                CharacterId = charId,
                RunSeed = "SEED" + new Random().Next(1000, 9999),
                FloorNumber = 1,
                Score = 0,
                IsCompleted = false,
                TotalTimeSeconds = 0
            };

            var result = await _efRepo.CreateRunAsync(run);
            Console.WriteLine($"Забег создан с ID: {result.Id}");
        }

        private static async Task AddItemToRunEf()
        {
            var runs = await _efRepo!.GetAllRunsAsync();
            Console.WriteLine("Доступные забеги:");
            foreach (var r in runs)
            {
                Console.WriteLine($"  {r.Id}: {r.RunSeed} (персонаж: {r.Character?.Name ?? "Unknown"})");
            }

            Console.Write("ID забега: ");
            if (!Guid.TryParse(Console.ReadLine(), out var runId))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            var items = await _efRepo.GetAllItemsAsync();
            Console.WriteLine("Доступные предметы:");
            foreach (var i in items)
            {
                Console.WriteLine($"  {i.Id}: {i.Name}");
            }

            Console.Write("ID предмета: ");
            if (!Guid.TryParse(Console.ReadLine(), out var itemId))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            Console.Write("Порядок подбора (число): ");
            int.TryParse(Console.ReadLine(), out var order);

            var result = await _efRepo.AddItemToRunAsync(runId, itemId, order > 0 ? order : null);
            Console.WriteLine($"Предмет добавлен в забег с ID: {result.Id}");
        }

        // ============= Сложные запросы =============
        private static async Task ComplexQueriesMenu()
        {
            // Обновляем EfRepository для этого запроса
            using var scope = _serviceProvider!.CreateScope();
            _efRepo = scope.ServiceProvider.GetRequiredService<EfRepository>();

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║     СЛОЖНЫЕ ЗАПРОСЫ                                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");

            try
            {
                Console.WriteLine("\n=== Статистика персонажей (EF) ===");
                var stats = await _efRepo!.GetCharacterStatsAsync();
                foreach (var s in stats)
                {
                    Console.WriteLine($"  {s.CharacterName}: {s.RunsCount} забегов, " +
                                      $"средний счёт: {s.AverageScore:F2}, " +
                                      $"макс: {s.MaxScore:F2}, " +
                                      $"средний этаж: {s.AverageFloor:F1}");
                }

                Console.WriteLine("\n=== Статистика предметов (EF) ===");
                var itemStats = await _efRepo.GetItemUsageStatsAsync();
                foreach (var s in itemStats.Take(10))
                {
                    Console.WriteLine($"  {s.ItemName} ({s.ItemType}): использован {s.UsageCount} раз, " +
                                      $"средний бонус урона: {s.AvgDamageBonus:+0.00;-0.00}");
                }

                Console.WriteLine("\n=== Детали забегов (EF) ===");
                var runDetails = await _efRepo.GetRunDetailsAsync();
                foreach (var r in runDetails)
                {
                    Console.WriteLine($"  {r.CharacterName}: этаж {r.FloorNumber}, " +
                                      $"счёт {r.Score:F2}, {r.ItemsCount} предметов, " +
                                      $"время {r.TotalTimeMinutes} мин, " +
                                      $"суммарный бонус урона: {r.TotalDamageBonus:+0.00;-0.00}");
                }

                Console.WriteLine("\n=== Статистика персонажей (ADO.NET) ===");
                var adoStats = await _adoRepo!.GetCharacterStatisticsAsync();
                foreach (var s in adoStats)
                {
                    Console.WriteLine($"  {s.CharacterName}: {s.RunsCount} забегов, " +
                                      $"средний счёт: {s.Averagescore:F2}, " +
                                      $"макс: {s.Maxscore:F2}, " +
                                      $"средний этаж: {s.Averagefloor:F1}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // ============= Тестовые данные =============
        private static async Task InsertTestData()
        {
            Console.WriteLine("Вставка тестовых данных...");

            try
            {
                // Проверяем, есть ли уже данные
                var existingCharacters = await _adoRepo!.GetAllCharactersAsync();
                if (existingCharacters.Any())
                {
                    Console.WriteLine("Данные уже существуют. Пропускаем вставку.");
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                    return;
                }

                var characters = new[]
                {
                    new Character { Name = "Isaac", Health = 100, Damage = 10.5m, Speed = 1.2m, IsUnlocked = true },
                    new Character { Name = "Magdalene", Health = 120, Damage = 8m, Speed = 0.9m, IsUnlocked = true },
                    new Character { Name = "Cain", Health = 80, Damage = 12m, Speed = 1.4m, IsUnlocked = true },
                    new Character { Name = "Judas", Health = 70, Damage = 15m, Speed = 1.1m, IsUnlocked = true },
                };

                foreach (var c in characters)
                {
                    await _adoRepo.CreateCharacterAsync(c);
                }

                var items = new[]
                {
                    new Item { Name = "Brimstone", Description = "Blood laser", ItemType = "active", 
                               DamageBonus = 20, HealthBonus = 0, SpeedBonus = 0, IsCursed = false },
                    new Item { Name = "Sacred Heart", Description = "Massive damage up", ItemType = "passive",
                               DamageBonus = 25, HealthBonus = 10, SpeedBonus = -0.2m, IsCursed = false },
                    new Item { Name = "Soy Milk", Description = "Tears up, damage down", ItemType = "passive",
                               DamageBonus = -5, HealthBonus = 0, SpeedBonus = 0, IsCursed = true },
                };

                foreach (var i in items)
                {
                    await _adoRepo.CreateItemAsync(i);
                }

                Console.WriteLine("Тестовые данные успешно добавлены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при вставке данных: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private static async Task ClearAllData()
        {
            Console.Write("Вы уверены, что хотите удалить все данные? (y/n): ");
            var confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y") return;

            try
            {
                using var context = new ApplicationDbContext();
                
                // Удаляем в правильном порядке из-за внешних ключей
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE run_items CASCADE");
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE runs CASCADE");
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE characters CASCADE");
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE items CASCADE");
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE enemies CASCADE");

                Console.WriteLine("Все данные удалены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при очистке: {ex.Message}");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }
}
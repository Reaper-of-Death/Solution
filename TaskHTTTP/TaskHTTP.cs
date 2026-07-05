using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace CurrencyAndEmployeeApi
{
    // Классы для десериализации ответа от API сотрудников
    public class Employee
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("employee_name")]
        public string EmployeeName { get; set; }

        // Используем string вместо int, так как API возвращает строку
        [JsonPropertyName("employee_salary")]
        public string EmployeeSalary { get; set; }

        [JsonPropertyName("employee_age")]
        public string EmployeeAge { get; set; }

        [JsonPropertyName("profile_image")]
        public string ProfileImage { get; set; }
    }

    public class EmployeeResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("data")]
        public List<Employee> Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    // Класс для создания нового сотрудника (POST)
    public class CreateEmployeeRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("salary")]
        public string Salary { get; set; }

        [JsonPropertyName("age")]
        public string Age { get; set; }
    }

    public class CreateEmployeeResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("data")]
        public CreateEmployeeData Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class CreateEmployeeData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("salary")]
        public string Salary { get; set; }

        [JsonPropertyName("age")]
        public string Age { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Демонстрация работы с API ===\n");

            // Регистрируем кодировку для Windows-1251
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                // 1. Получение курсов валют от ЦБ РФ
                await GetCurrencyRates("14/01/2025");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при получении курсов валют: {ex.Message}");
            }

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Небольшая задержка перед следующим запросом
            await Task.Delay(1000);

            try
            {
                // 2. Получение списка сотрудников из фиктивного API
                await GetEmployees();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при получении списка сотрудников: {ex.Message}");
            }

            Console.WriteLine("\n" + new string('-', 60) + "\n");

            // Задержка перед POST запросом
            await Task.Delay(2000);

            try
            {
                // 3. Создание нового сотрудника
                await CreateEmployee("Иван Петров", "120000", "35");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при создании сотрудника: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Получение и вывод курсов валют на указанную дату
        /// </summary>
        static async Task GetCurrencyRates(string date)
        {
            Console.WriteLine($"1. Запрос курсов валют на дату: {date}");
            string url = $"https://www.cbr.ru/scripts/XML_daily.asp?date_req={date}";

            // Получаем ответ в виде массива байт
            byte[] responseBytes = await client.GetByteArrayAsync(url);
            
            // Используем Windows-1251 для декодирования
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            string xmlContent = win1251.GetString(responseBytes);

            // Парсинг XML
            XDocument xmlDoc = XDocument.Parse(xmlContent);
            XElement root = xmlDoc.Root;

            Console.WriteLine($"Дата: {root.Attribute("Date")?.Value}");
            Console.WriteLine($"Название: {root.Attribute("name")?.Value}");
            Console.WriteLine("\nКурсы валют:");

            var valutes = root.Elements("Valute").ToList();
            int count = 0;
            foreach (var valute in valutes)
            {
                if (count++ >= 5)
                {
                    Console.WriteLine($"... и еще {valutes.Count - 5} валют");
                    break;
                }

                string charCode = valute.Element("CharCode")?.Value;
                string nominal = valute.Element("Nominal")?.Value;
                string name = valute.Element("Name")?.Value;
                string value = valute.Element("Value")?.Value;

                Console.WriteLine($"  {charCode} ({nominal} {name}): {value} руб.");
            }

            Console.WriteLine($"\n✅ Получено {valutes.Count} курсов валют");
        }

        /// <summary>
        /// Получение списка всех сотрудников из фиктивного API
        /// </summary>
        static async Task GetEmployees()
        {
            Console.WriteLine("2. Запрос списка всех сотрудников");
            string url = "https://dummy.restapiexample.com/api/v1/employees";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            EmployeeResponse result = JsonSerializer.Deserialize<EmployeeResponse>(jsonResponse, options);

            if (result != null && result.Status == "success")
            {
                Console.WriteLine($"Статус: {result.Status}");
                Console.WriteLine($"Количество сотрудников: {result.Data?.Count ?? 0}");
                Console.WriteLine("\nПервые 5 сотрудников:");

                int count = 0;
                if (result.Data != null)
                {
                    foreach (var emp in result.Data)
                    {
                        if (count++ >= 5)
                        {
                            Console.WriteLine($"... и еще {result.Data.Count - 5} сотрудников");
                            break;
                        }
                        Console.WriteLine($"  ID: {emp.Id}, Имя: {emp.EmployeeName}, " +
                                          $"Зарплата: {emp.EmployeeSalary} руб., Возраст: {emp.EmployeeAge}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Ошибка: {result?.Message ?? "Неизвестная ошибка"}");
            }
        }

        /// <summary>
        /// Создание нового сотрудника через POST-запрос
        /// </summary>
        static async Task CreateEmployee(string name, string salary, string age)
        {
            Console.WriteLine("3. Создание нового сотрудника");
            string url = "https://dummy.restapiexample.com/api/v1/create";

            var newEmployee = new CreateEmployeeRequest
            {
                Name = name,
                Salary = salary,
                Age = age
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string jsonRequest = JsonSerializer.Serialize(newEmployee, options);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            Console.WriteLine($"Отправка данных: {jsonRequest}");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                
                // Проверяем статус ответа
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("⏳ Сервер говорит: слишком много запросов. Подождите 5 секунд...");
                    await Task.Delay(25000);
                    
                    // Повторяем запрос
                    response = await client.PostAsync(url, content);
                }
                
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                CreateEmployeeResponse result = JsonSerializer.Deserialize<CreateEmployeeResponse>(jsonResponse, options);

                if (result != null && result.Status == "success")
                {
                    Console.WriteLine($"✅ Сотрудник успешно создан!");
                    Console.WriteLine($"   ID: {result.Data?.Id}");
                    Console.WriteLine($"   Имя: {result.Data?.Name}");
                    Console.WriteLine($"   Зарплата: {result.Data?.Salary} руб.");
                    Console.WriteLine($"   Возраст: {result.Data?.Age} лет");
                    Console.WriteLine($"   Сообщение: {result.Message}");
                }
                else
                {
                    Console.WriteLine($"❌ Ошибка при создании: {result?.Message ?? "Неизвестная ошибка"}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Ошибка HTTP: {ex.Message}");
                Console.WriteLine("💡 Попробуйте запустить программу позже, когда API снова станет доступным.");
            }
        }
    }
}

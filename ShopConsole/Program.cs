using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Shop.Common;
using Shop.Infrastructure;

namespace Shop.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Налаштовуємо кодування, щоб консоль коректно виводила український текст
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ЗАПУСК ЛАБОРАТОРНИХ РОБІТ №1, №2, №3 ===");

            using var context = new ShopContext();

            // Автоматично створюємо базу даних SQLite, якщо файлу shop.db ще немає
            Console.WriteLine("Перевірка та створення бази даних SQLite...");
            await context.Database.EnsureCreatedAsync();

            var service = new OrderCrudService(context);

            // Безпечна для потоків колекція (Вимога Лаб №2)
            var threadSafeBag = new ConcurrentBag<Order>();

            Console.WriteLine("Лаб №2: Генерація 1000 замовлень у багатопотоковому режимі...");

            // Паралельний цикл створення об'єктів у багатопотоці (Вимога Лаб №2)
            Parallel.For(0, 1000, i =>
            {
                threadSafeBag.Add(Order.CreateNew()); // Виклик статичного методу з Лаб №2
            });

            Console.WriteLine($"Успішно згенеровано {threadSafeBag.Count} об'єктів у пам'яті.");
            Console.WriteLine("Лаб №3: Збереження замовлень у базу даних через Репозиторій...");

            // Переносимо дані з доменних моделей у моделі бази даних та зберігаємо
            foreach (var order in threadSafeBag)
            {
                await service.CreateAsync(new OrderModel
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    CustomerEmail = order.CustomerEmail
                });
            }

            Console.WriteLine("Дані успішно записані в файл shop.db!");

            // Демонстрація роботи пагінації за допомогою LINQ (Вимога Лаб №2 та №3)
            Console.WriteLine("\nЛаб №3: Тест пагінації. Читаємо перші 5 замовлень із бази (Сторінка 1):");
            var firstPage = await service.ReadAllAsync(1, 5);

            foreach (var o in firstPage)
            {
                Console.WriteLine($"[Замовлення] ID: {o.Id} | Дата: {o.OrderDate} | Email: {o.CustomerEmail}");
            }

            Console.WriteLine("\n=======================================================");
            Console.WriteLine("Усі три лабораторні роботи успішно виконані та об'єднані!");
            Console.WriteLine("=======================================================");
        }
    }
}
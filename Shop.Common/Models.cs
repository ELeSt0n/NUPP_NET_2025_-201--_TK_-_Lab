using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Common
{
    // === ЛАБОРАТОРНА РОБОТА №1 ===
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Виправлено варнінг
        public decimal Price { get; set; }
    }

    public class DigitalProduct : Product
    {
        public string DownloadUrl { get; set; } = string.Empty; // Виправлено варнінг
    }

    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail { get; set; } = string.Empty; // Виправлено варнінг

        // === ЛАБОРАТОРНА РОБОТА №2 ===
        public static Order CreateNew()
        {
            var random = new Random();
            return new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                CustomerEmail = $"customer{random.Next(1, 500)}@poltava.ua"
            };
        }
    }

    // === ЛАБОРАТОРНА РОБОТА №2 та №3 ===
    public interface ICrudServiceAsync<T>
    {
        Task<bool> CreateAsync(T element);
        Task<T> ReadAsync(Guid id);
        Task<IEnumerable<T>> ReadAllAsync();
        Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
        Task<bool> UpdateAsync(T element);
        Task<bool> RemoveAsync(T element);
        Task<bool> SaveAsync();
    }
}
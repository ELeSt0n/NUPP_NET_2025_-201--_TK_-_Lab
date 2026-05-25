using Microsoft.EntityFrameworkCore;
using Shop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Infrastructure
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; } = null!;
    }

    public class DigitalProductModel : ProductModel
    {
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }

    public class OrderModel
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class ShopContext : DbContext
    {
        public DbSet<ProductModel> Products => Set<ProductModel>();
        public DbSet<DigitalProductModel> DigitalProducts => Set<DigitalProductModel>();
        public DbSet<CategoryModel> Categories => Set<CategoryModel>();
        public DbSet<OrderModel> Orders => Set<OrderModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=shop.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>().ToTable("Products");
            modelBuilder.Entity<DigitalProductModel>().ToTable("DigitalProducts");

            modelBuilder.Entity<CategoryModel>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
        }
    }

    public class OrderRepository
    {
        private readonly ShopContext _context;
        public OrderRepository(ShopContext context) => _context = context;

        public async Task AddAsync(OrderModel entity) => await _context.Orders.AddAsync(entity);
        public async Task<OrderModel?> GetByIdAsync(Guid id) => await _context.Orders.FindAsync(id);
        public async Task<IEnumerable<OrderModel>> GetAllAsync() => await _context.Orders.ToListAsync();
        public void Update(OrderModel entity) => _context.Orders.Update(entity);
        public void Delete(OrderModel entity) => _context.Orders.Remove(entity);
    }

    public class OrderCrudService : ICrudServiceAsync<OrderModel>
    {
        private readonly ShopContext _context;
        private readonly OrderRepository _repository;

        public OrderCrudService(ShopContext context)
        {
            _context = context;
            _repository = new OrderRepository(_context);
        }

        public async Task<bool> CreateAsync(OrderModel element) { await _repository.AddAsync(element); return await SaveAsync(); }

        public async Task<OrderModel> ReadAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return result ?? new OrderModel();
        }

        public async Task<IEnumerable<OrderModel>> ReadAllAsync() => await _repository.GetAllAsync();

        public async Task<IEnumerable<OrderModel>> ReadAllAsync(int page, int amount) =>
            await _context.Orders.OrderBy(o => o.OrderDate).Skip((page - 1) * amount).Take(amount).ToListAsync();

        public async Task<bool> UpdateAsync(OrderModel element) { _repository.Update(element); return await SaveAsync(); }
        public async Task<bool> RemoveAsync(OrderModel element) { _repository.Delete(element); return await SaveAsync(); }
        public async Task<bool> SaveAsync() => await _context.SaveChangesAsync() > 0;
    }
}
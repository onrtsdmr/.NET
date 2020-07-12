using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.dataaccess.Concrete.EfCore
{
    public class ShopContext: DbContext
    {
        public DbSet<Product> Products {get; set;}
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder  optionsBuilder)
        {
            optionsBuilder.UseMySql(@"server=localhost;port=3306;database=ShopAppDB;user=root;password=onur123123");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
            .HasKey(c => new {c.CategoryID, c.ProductId});
        }
    }
}
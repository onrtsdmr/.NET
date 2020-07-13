using System.Linq;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.dataaccess.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (!context.Database.GetPendingMigrations().Any())
            {
                // if (context.Categories.Count() == 0)
                // {
                //     context.Categories.AddRange(Categories);
                // }


                // context.Products.AddRange(Products);
                // context.AddRange(ProductCategories);
            }

            context.SaveChanges();
        }

        private static Category[] Categories =
        {
            new Category() {Name = "Telefon", Url = "/products/telefon"},
            new Category() {Name = "Bilgisayar", Url = "/products/bilgisayar"},
            new Category() {Name = "Elektronik", Url = "/products/elektronik"},
            new Category() {Name = "Beyaz Eşya", Url = "/products/beyaz-esya"},
        };

        private static Product[] Products =
        {
            new Product()
            {
                Name = "Samsung S5", Price = 2000, ImageUrl = "2.png", Description = "İyi Telefon", IsApproved = true,
                Url = "samsung-s5"
            },
            new Product()
            {
                Name = "Xiaomi Redmi Note 8", Price = 2200, ImageUrl = "1.png",
                Description = "Hem ucuz hem de performanslı", IsApproved = true, Url = "xiaomi-redmi-note-8"
            },
            new Product()
            {
                Name = "IPhone 7 Plus", Price = 7000, ImageUrl = "4.png", Description = "Muazzam", IsApproved = true,
                Url = "iphone-7-plus"
            },
            new Product()
            {
                Name = "IPhone 7", Price = 6000, ImageUrl = "3.png", Description = "Güzel", IsApproved = true,
                Url = "iphone-7"
            },
            new Product()
            {
                Name = "IPhone 6", Price = 5000, ImageUrl = "3.png", Description = "Güzel", IsApproved = false,
                Url = "iphone-6"
            },
        };

        private static object[] ProductCategories =
        {
            new ProductCategory() {Product = Products[0], CategoryID = 1},
            new ProductCategory() {Product = Products[0], CategoryID = 3},
            new ProductCategory() {Product = Products[1], CategoryID = 1},
            new ProductCategory() {Product = Products[1], CategoryID = 3},
            new ProductCategory() {Product = Products[2], CategoryID = 1},
            new ProductCategory() {Product = Products[2], CategoryID = 3},
            new ProductCategory() {Product = Products[3], CategoryID = 1},
            new ProductCategory() {Product = Products[3], CategoryID = 3},
            new ProductCategory() {Product = Products[4], CategoryID = 1},
            new ProductCategory() {Product = Products[4], CategoryID = 3},
        };
    }
}
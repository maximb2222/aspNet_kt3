using aspNet_kt3.Models;

namespace aspNet_kt3.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic items" },
                    new Category { Name = "Clothing", Description = "Clothing items" },
                    new Category { Name = "Books", Description = "Books and magazines" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Laptop", Description = "High performance laptop", Price = 999.99m, CategoryId = 1 },
                    new Product { Name = "Smartphone", Description = "Latest smartphone", Price = 699.99m, CategoryId = 1 },
                    new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, CategoryId = 2 },
                    new Product { Name = "Jeans", Description = "Blue jeans", Price = 49.99m, CategoryId = 2 },
                    new Product { Name = "Programming Book", Description = "C# programming", Price = 39.99m, CategoryId = 3 }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
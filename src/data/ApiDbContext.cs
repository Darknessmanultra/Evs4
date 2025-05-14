using Bogus;
using EVS4.src.models;
using Microsoft.EntityFrameworkCore;

namespace EVS4.src.data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<Producto> Productos {get;set;}
    }
    public static class DbSeeder
    {
        public static void Seed(ApiDbContext context)
        {
            if (context.Productos.Any())
                return;

            var faker = new Faker<Producto>()
                .RuleFor(p => p.SKU, f => f.Commerce.Ean13().Substring(0, 8).ToUpper())
                .RuleFor(p => p.Nombre, f => f.Commerce.ProductName())
                .RuleFor(p => p.Stock, f => f.Random.Int(0,100))
                .RuleFor(p => p.Precio, f => f.Random.Int(1,8000))
                .RuleFor(p => p.Activo, _ => true);

            var products = new HashSet<string>();
            var uniqueProducts = new List<Producto>();

            while (uniqueProducts.Count < 51)
            {
                var p = faker.Generate();
                if (products.Add(p.SKU))
                    uniqueProducts.Add(p);
            }

            context.Productos.AddRange(uniqueProducts);
            context.SaveChanges();
        }
    }
}
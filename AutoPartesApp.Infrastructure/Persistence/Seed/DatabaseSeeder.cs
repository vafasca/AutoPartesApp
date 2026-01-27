using AutoPartesApp.Infrastructure.Persistence;
using AutoPartesApp.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AutoPartesDbContext context)
    {
        // 1️⃣ Usuarios
        if (!context.Users.Any())
        {
            var users = UserSeed.GetUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync(); // 👈 guardar aquí
        }

        // 2️⃣ Categorías
        if (!context.Categories.Any())
        {
            var categories = CategorySeed.GetCategories();
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync(); // 👈 guardar aquí
        }

        // 3️⃣ Productos
        if (!context.Products.Any())
        {
            var products = ProductSeed.GetProducts(context.Categories.ToList());
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync(); // 👈 guardar aquí
        }

        // 4️⃣ Órdenes
        if (!context.Orders.Any())
        {
            var orders = OrderSeed.GetOrders(context.Users.ToList(), context.Products.ToList());
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync(); // 👈 guardar aquí
        }

        // 5️⃣ Entregas
        if (!context.Deliveries.Any())
        {
            var deliveries = DeliverySeed.GetDeliveries(context.Orders.ToList(), context.Users.ToList());
            await context.Deliveries.AddRangeAsync(deliveries);
            await context.SaveChangesAsync(); // 👈 guardar aquí
        }
    }
}
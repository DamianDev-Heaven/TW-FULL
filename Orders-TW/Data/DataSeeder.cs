using Orders_TW.Data;
using OrdersTW.Models;

namespace Orders_TW.Data
{
    public static class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            if (context.Products.Any())
            {
                return; // Ya hay datos
            }

            var products = new List<Product>
            {
                new Product { Name = "Laptop HP EliteBook", Description = "Laptop profesional de alto rendimiento", Price = 1299.99m, Stock = 15 },
                new Product { Name = "Mouse Logitech MX Master", Description = "Mouse ergonómico inalámbrico", Price = 99.99m, Stock = 50 },
                new Product { Name = "Teclado Mecánico Corsair", Description = "Teclado mecánico RGB con switches Cherry MX", Price = 149.99m, Stock = 30 },
                new Product { Name = "Monitor Dell 27\" 4K", Description = "Monitor profesional UHD", Price = 599.99m, Stock = 20 },
                new Product { Name = "Webcam Logitech C920", Description = "Webcam Full HD 1080p", Price = 79.99m, Stock = 40 },
                new Product { Name = "Auriculares Sony WH-1000XM4", Description = "Auriculares con cancelación de ruido", Price = 349.99m, Stock = 25 },
                new Product { Name = "SSD Samsung 1TB", Description = "Disco sólido NVMe de alta velocidad", Price = 129.99m, Stock = 60 },
                new Product { Name = "RAM Corsair 16GB DDR4", Description = "Memoria RAM 3200MHz", Price = 89.99m, Stock = 45 },
                new Product { Name = "Silla Ergonómica Herman Miller", Description = "Silla de oficina premium", Price = 899.99m, Stock = 10 },
                new Product { Name = "Escritorio Ajustable", Description = "Escritorio eléctrico altura ajustable", Price = 499.99m, Stock = 12 }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            var customers = new List<Customer>
            {
                new Customer { FullName = "Juan Pérez", Email = "juan.perez@email.com" },
                new Customer { FullName = "María García", Email = "maria.garcia@email.com" },
                new Customer { FullName = "Carlos López", Email = "carlos.lopez@email.com" },
                new Customer { FullName = "Ana Martínez", Email = "ana.martinez@email.com" },
                new Customer { FullName = "Pedro Rodríguez", Email = "pedro.rodriguez@email.com" },
                new Customer { FullName = "Laura Fernández", Email = "laura.fernandez@email.com" },
                new Customer { FullName = "Miguel Sánchez", Email = "miguel.sanchez@email.com" },
                new Customer { FullName = "Isabel Ramírez", Email = "isabel.ramirez@email.com" }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();

            var orders = new List<Order>
            {
                new Order
                {
                    CustomerId = 1,
                    OrderDate = DateTime.UtcNow.AddDays(-10),
                    Total = 1399.98m,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 1299.99m },
                        new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 99.99m }
                    }
                },
                new Order
                {
                    CustomerId = 2,
                    OrderDate = DateTime.UtcNow.AddDays(-8),
                    Total = 749.98m,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 3, Quantity = 1, UnitPrice = 149.99m },
                        new OrderItem { ProductId = 4, Quantity = 1, UnitPrice = 599.99m }
                    }
                },
                new Order
                {
                    CustomerId = 3,
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    Total = 429.98m,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 6, Quantity = 1, UnitPrice = 349.99m },
                        new OrderItem { ProductId = 5, Quantity = 1, UnitPrice = 79.99m }
                    }
                },
                new Order
                {
                    CustomerId = 4,
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    Total = 219.98m,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 7, Quantity = 1, UnitPrice = 129.99m },
                        new OrderItem { ProductId = 8, Quantity = 1, UnitPrice = 89.99m }
                    }
                },
                new Order
                {
                    CustomerId = 5,
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    Total = 1399.98m,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 9, Quantity = 1, UnitPrice = 899.99m },
                        new OrderItem { ProductId = 10, Quantity = 1, UnitPrice = 499.99m }
                    }
                }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}

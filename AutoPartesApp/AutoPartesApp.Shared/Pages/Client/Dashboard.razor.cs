using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Client
{
    public partial class Dashboard : ComponentBase
    {
        private string searchQuery = string.Empty;
        private int cartItemsCount = 3;
        private string userVehicle = "2018 Toyota Corolla";

        private List<PromotionItem> promotions = new();
        private List<CategoryItem> categories = new();
        private List<ProductItem> recommendedProducts = new();

        protected override void OnInitialized()
        {
            LoadPromotions();
            LoadCategories();
            LoadRecommendedProducts();
        }

        private void LoadPromotions()
        {
            promotions = new List<PromotionItem>
            {
                new PromotionItem
                {
                    Id = 1,
                    Title = "Frenos de Alto Desempeño",
                    Description = "Hasta 30% desc. en kits Brembo",
                    Badge = "TIEMPO LIMITADO",
                    BadgeColor = "red-600",
                    GradientClass = "bg-gradient-to-r from-slate-900 to-slate-700",
                    ButtonClass = "bg-primary text-white",
                    ButtonText = "Comprar Ahora",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAnaUAC3nNCyCMS5_w8DwEXL0iGe3gl66Q4ep2z3jY0AYDTr-jQgKsE4JAoJoJaRwOTQyfa2SFQep5XsrCv1KCthgE5bvohWzVXeplJmy60WBt3zCf8Y8Vs7HFr-Qe_XDgInDQPEmdZd5QvwGIt74kge4gcmZKIvHJa6haY6UzgEYr4ivr7CjIe2rMTqvsm8Aoc2U1MxKP0dX20QofTw8Vf3a7a3XlKKE8GBPskajZZtmGp0B9xmd0d7aZqhWQZqM7Re1bMjmpPrcc"
                },
                new PromotionItem
                {
                    Id = 2,
                    Title = "Iluminación LED",
                    Description = "Envío gratis en luces",
                    Badge = "LIQUIDACIÓN",
                    BadgeColor = "primary",
                    GradientClass = "bg-gradient-to-r from-blue-900 to-indigo-900",
                    ButtonClass = "bg-white text-slate-900",
                    ButtonText = "Explorar",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBmbr7HSS8mkXQyqbdcnbyiLJmTpNRnsiyWYKDJXSnrzkdn4hUHfyxBKkWwPi4Yk9ZI-9P1m2HVQGSdT4DOKn5idQKB7yi4vr09CBFrWcw8MDxRXVxW9isfr_qxYvQuCITeMKR8e4WchH6ssXLjHOehCKJqrtIaVBNmkseoIWlWTHYjFdE5yjHkkiI5P26h_Z_jJwA0WGv2g9k27lI9zD8xJaRwJGdXP-5fJrj68VFLCCNoLLfJjfGgNwO7ydH1qmbsBrDBQg1KNjg"
                }
            };
        }

        private void LoadCategories()
        {
            categories = new List<CategoryItem>
            {
                new CategoryItem { Id = 1, Name = "Motores", Icon = "earth_engine", BgClass = "bg-blue-100 dark:bg-blue-900/30", TextClass = "text-primary" },
                new CategoryItem { Id = 2, Name = "Frenos", Icon = "settings_input_component", BgClass = "bg-red-100 dark:bg-red-900/30", TextClass = "text-red-600" },
                new CategoryItem { Id = 3, Name = "Luces", Icon = "lightbulb", BgClass = "bg-yellow-100 dark:bg-yellow-900/30", TextClass = "text-yellow-600" },
                new CategoryItem { Id = 4, Name = "Eléctrico", Icon = "battery_charging_full", BgClass = "bg-green-100 dark:bg-green-900/30", TextClass = "text-green-600" },
                new CategoryItem { Id = 5, Name = "Clima", Icon = "ac_unit", BgClass = "bg-purple-100 dark:bg-purple-900/30", TextClass = "text-purple-600" },
                new CategoryItem { Id = 6, Name = "Llantas", Icon = "tire_repair", BgClass = "bg-orange-100 dark:bg-orange-900/30", TextClass = "text-orange-600" }
            };
        }

        private void LoadRecommendedProducts()
        {
            recommendedProducts = new List<ProductItem>
            {
                new ProductItem
                {
                    Id = 1,
                    Name = "Filtro de Aire Premium - Alto Flujo",
                    Sku = "KN-12044-T",
                    Price = 45.99m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCfeqZOOmLto5AuFqiHyCkVj5jITAfAiump3dSS40ePery4jUahRV63eS3CzHfXe6YXoqhN7gxm6L--BbiG4AoBfwu_wBTXxrZQisNeVGAaP7O4chDg2CEPqII0aK3iwC3qFtH2KXUi3s6CUyO4k-CMK7_pTfxKl3pTRbUBSZDsquT1nu5uxIMu89yQjIq_Fx4dT51mXxxQtJA3uHvyMUk_8Mt2LpLZNKBXIP-HTBXj1LFaOznpYS9ndnazVkR3m_uRMyg2z98OWMc"
                },
                new ProductItem
                {
                    Id = 2,
                    Name = "Bujías Platinum (Set de 4)",
                    Sku = "BSH-PLAT-77",
                    Price = 28.50m,
                    OriginalPrice = 32.00m,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCTgu4qU0-MqiiKPlssEzEySBEAw2_9LTlxPCKJFbTlL-gC5BFCRc8Dg4PCQNcImkiwOlYElVbyiXYt_-KGG1tgMGg3kyyDjRegOWitVQpk-z8P7TW53Qu70XNUrgkasW2q-m9URcwpRufC0dMK07B37iMkCubsfMwqxqrer8V-HwaFp0PGriYlAAnjUGITCm-k-x65OZFEXEjAwjF80iDN4VEEWw02D8qUSzo15LO61UK5BboFCXfkqR6yCJfTbyA6NXYax3hSS_Y"
                }
            };
        }

        private void ChangeVehicle()
        {
            Console.WriteLine("Cambiar vehículo clicked");
            // Implementar lógica para cambiar vehículo
        }

        private void ViewPromotion(int id)
        {
            Console.WriteLine($"Ver promoción: {id}");
            // Navegar a detalles de promoción
        }

        private void ViewAllCategories()
        {
            Console.WriteLine("Ver todas las categorías");
            // Navegar a página de categorías
        }

        private void ViewCategory(int id)
        {
            Console.WriteLine($"Ver categoría: {id}");
            // Navegar a productos de la categoría
        }

        private void RefreshRecommendations()
        {
            Console.WriteLine("Actualizar recomendaciones");
            // Recargar productos recomendados
            LoadRecommendedProducts();
        }

        private void AddToCart(int productId)
        {
            Console.WriteLine($"Agregar al carrito: {productId}");
            cartItemsCount++;
            StateHasChanged();
        }

        // Clases internas para los datos
        private class PromotionItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Badge { get; set; } = string.Empty;
            public string BadgeColor { get; set; } = string.Empty;
            public string GradientClass { get; set; } = string.Empty;
            public string ButtonClass { get; set; } = string.Empty;
            public string ButtonText { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
        }

        private class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string BgClass { get; set; } = string.Empty;
            public string TextClass { get; set; } = string.Empty;
        }

        private class ProductItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Sku { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal? OriginalPrice { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}

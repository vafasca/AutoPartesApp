using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Client
{
    public partial class Catalog : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private string searchQuery = string.Empty;
        private string selectedCategory = "Todos";
        private string? selectedBrand = null;
        private string? selectedYear = null;
        private bool hasNewNotifications = true;
        private int cartItemsCount = 3;
        private bool isLoading = false;
        private bool[] showFilterDropdown = new bool[3]; // Marca, Año, Categoría

        // Filter Options
        private List<string> brands = new() { "Todas", "Toyota", "Honda", "Ford", "Chevrolet", "Nissan" };
        private List<string> years = new() { "Todos", "2024", "2023", "2022", "2021", "2020", "2019", "2018" };
        private List<string> categories = new() { "Todos", "Motores", "Frenos", "Suspensión", "Eléctrico", "Filtros", "Llantas" };

        // Products Data
        private List<ProductItem> allProducts = new();
        private List<ProductItem> filteredProducts = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            isLoading = true;
            StateHasChanged();

            // Simular carga de datos
            await Task.Delay(500);

            allProducts = new List<ProductItem>
            {
                new ProductItem
                {
                    Id = 1,
                    Name = "Bomba de Agua - Toyota Hilux",
                    Compatibility = "Modelos 2018 - 2022",
                    Price = 45.00m,
                    StockStatus = "Disponible",
                    Category = "Motores",
                    Brand = "Toyota",
                    Year = "2018-2022",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAQcUrF5mNw8ceq9DzvjGcPKcU4yIX65_qlbpUUOjnq77b6rnINdbdO5JZ45FLV91MyyQCJnTUhlGpDZH3xJXzkeEzgzQA-P0yYaNkCIliJ-Z0TJDoE6IczJ7mlK8uydaX8LyD4RVcAoXHuoim5OhvrpZezB1uUsN484f8gocSX8YTAud_4JPcRwyFjLSktwfaYmCowTOKjl_XWyrwc-hBTHlKs4EvEpD9TS8KXbHqThUdYByTwzXDzuBPOMh1HfT82zGT_5dSOEec",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 2,
                    Name = "Pastillas de Freno Ceramic",
                    Compatibility = "Ford Ranger / Raptor",
                    Price = 32.50m,
                    StockStatus = "Bajo Stock",
                    Category = "Frenos",
                    Brand = "Ford",
                    Year = "2019-2023",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBQGBq5gUDbPJg2ZzHydI217J4h46Mt1zAg9qHHR0ZjaVe9f5bsZ4BnFUdC5QtPSWkSiPrddlaugkOArxyQLxQp8zOLd_63oUCv6tiHPIxqsZTpJ_lj2Vl1KfroKWgGfUYuaD2CEe09a0RvgbYXQL6on0K_kPNzYfD9_VhQRJZtxZ_2e9cEMBQ61dIV1zZv4uPt05liZ3gvWwQoLPsEMDkE-YElHk9i5JCdUs-AQlX0krkjyOeU7oWiU0gVoz3L8EzYermoubszSWk",
                    IsFavorite = true
                },
                new ProductItem
                {
                    Id = 3,
                    Name = "Filtro de Aceite Sintético",
                    Compatibility = "Universal - Kit 3 piezas",
                    Price = 12.00m,
                    StockStatus = "Disponible",
                    Category = "Filtros",
                    Brand = "Universal",
                    Year = "Todos",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuACc5moWRM2H2Gn5bMEezMvvvd_OO5QJNhSsCvDJpMRpcRnJRE5DTm-bsP7GOcEMCG1-o5M-Yj9MGyj-wrShZTPQmCFjfiXR3A_EwwxbN4jh_L7ro_f6fW3BlnhOBP2OCL6dxiFnpRXPujyK1roxsd37d8Dn6NJQPFkdoax0uUQ3OTtLN-CbhmS5tc2qRzr9DlYVIsGfwhA49JMldv4exUrjzeoUPUy7bE3Hw3uUTbwQCoUj6ztsA22QT0EFSUDqpl8BzgQllfbFEg",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 4,
                    Name = "Amortiguador Delantero HD",
                    Compatibility = "Chevrolet S10 / Trailblazer",
                    Price = 85.00m,
                    StockStatus = "Disponible",
                    Category = "Suspensión",
                    Brand = "Chevrolet",
                    Year = "2020-2024",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuALurHP4v9JWCWLDNu7Sw6PXTFK4uomhGlB1Q2cjPP-HR0d_bJISMLsEWMJFcsVg9lIFQLFw-_KOblp0wikh7_fZuAT_qnHY2wF5jowJL-Gtabkiq5gGv0P8p7wMc7F2SfLH1q1WljWw5Qn8sTOObfBZC0njDHSXDdGtSsrQFpLudXOmJGMkqRz1Xo9u4BTeIbP5k4FpJtb4BOQrvYpw4u50TPM2U38F0uFRMomFF8IQ1CZzJeC6l5ito1RFfT6oPg6DMAfbWxX-HY",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 5,
                    Name = "Batería AGM 60Ah",
                    Compatibility = "Honda Civic / Accord",
                    Price = 120.00m,
                    OriginalPrice = 150.00m,
                    StockStatus = "Disponible",
                    Category = "Eléctrico",
                    Brand = "Honda",
                    Year = "2018-2023",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCfeqZOOmLto5AuFqiHyCkVj5jITAfAiump3dSS40ePery4jUahRV63eS3CzHfXe6YXoqhN7gxm6L--BbiG4AoBfwu_wBTXxrZQisNeVGAaP7O4chDg2CEPqII0aK3iwC3qFtH2KXUi3s6CUyO4k-CMK7_pTfxKl3pTRbUBSZDsquT1nu5uxIMu89yQjIq_Fx4dT51mXxxQtJA3uHvyMUk_8Mt2LpLZNKBXIP-HTBXj1LFaOznpYS9ndnazVkR3m_uRMyg2z98OWMc",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 6,
                    Name = "Kit de Embrague Completo",
                    Compatibility = "Nissan Frontier 2.5L",
                    Price = 195.00m,
                    StockStatus = "Bajo Stock",
                    Category = "Motores",
                    Brand = "Nissan",
                    Year = "2019-2022",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCTgu4qU0-MqiiKPlssEzEySBEAw2_9LTlxPCKJFbTlL-gC5BFCRc8Dg4PCQNcImkiwOlYElVbyiXYt_-KGG1tgMGg3kyyDjRegOWitVQpk-z8P7TW53Qu70XNUrgkasW2q-m9URcwpRufC0dMK07B37iMkCubsfMwqxqrer8V-HwaFp0PGriYlAAnjUGITCm-k-x65OZFEXEjAwjF80iDN4VEEWw02D8qUSzo15LO61UK5BboFCXfkqR6yCJfTbyA6NXYax3hSS_Y",
                    IsFavorite = true
                },
                new ProductItem
                {
                    Id = 7,
                    Name = "Neumáticos All-Terrain 265/70R16",
                    Compatibility = "Toyota 4Runner / Tacoma",
                    Price = 145.00m,
                    StockStatus = "Disponible",
                    Category = "Llantas",
                    Brand = "Toyota",
                    Year = "2018-2024",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB6uZF50tsprRl4k5YWsf49JRa5sdX2rKWTwPD5A5Zf8XM3h-AQ5vQ9Xcjmwh3jGx0JJnXGSi8MujD-l0sB256gKOXrRIWcqY1lUX-VLalWmR_fuXLAWy4dqnN_tslyEPtWyzOGQKCS_dYgnoiP8lr3pFElzQX7THVvt7bC4rnl3PGsyVbLFEjNK2Yo8UQ3pdsjV1peap3f4fv6MF1-uHuErmnnrbrpV968f2lfWxdDPL6rWxPIgA-TnU2qVK6q4DeD8QZ9F331gYI",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 8,
                    Name = "Discos de Freno Ventilados",
                    Compatibility = "Ford Escape / Edge",
                    Price = 78.50m,
                    OriginalPrice = 95.00m,
                    StockStatus = "Disponible",
                    Category = "Frenos",
                    Brand = "Ford",
                    Year = "2020-2024",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAS0wwKKyblieQqaAiJ0U3C9ZoXelcR_n_rw9Q6wltFXj0TbCc9x8ptW3o4wi0PM3hbiECnS9cwTqGv8yrO2A9G7w-UT5N4KRAn54nHtKOVbT3W7Oxrf2dt5LChUhRebLHoDLW_Cka9GninL6nmVBugw8sNzpGtOurtQQavDn2fa0ZIWvPXUb0STuUB-ASgh2cqz2qYFp0XrzZqrXuGkgOzQf9KYFGseQYHVu4WrjSCRlan5YXNAIU0xUO7d2lYif6puI53LFEeGvs",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 9,
                    Name = "Bujías Iridium (Set de 4)",
                    Compatibility = "Honda CR-V 2.4L",
                    Price = 42.00m,
                    StockStatus = "Disponible",
                    Category = "Motores",
                    Brand = "Honda",
                    Year = "2019-2023",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCpz8AO-Ul5-YrOvF9hRkADcphu0kwil3Of6V230a7QBxkyqrk4-WvbPBz1Vs_lgDa4orS9bJVqsYKJNCQ_c5UjYIap1GyJHVIJvyX2wsBm9YPMEX1lyV61csjqIjocEuebODFYUEKwUJJYsAhYeoSuWe35Ihsc_y47V5qz-WpoWPRmLSF3YmTSwJv7L2xCynqM49xqTV31Eyv5otit8a1DbklYZtJzH7OkcbPHg0eMtuE00XnrzX7qucWc4NrlAeCw-rekUW-g7uc",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 10,
                    Name = "Alternador 140A",
                    Compatibility = "Chevrolet Silverado",
                    Price = 165.00m,
                    StockStatus = "Agotado",
                    Category = "Eléctrico",
                    Brand = "Chevrolet",
                    Year = "2018-2022",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAQcUrF5mNw8ceq9DzvjGcPKcU4yIX65_qlbpUUOjnq77b6rnINdbdO5JZ45FLV91MyyQCJnTUhlGpDZH3xJXzkeEzgzQA-P0yYaNkCIliJ-Z0TJDoE6IczJ7mlK8uydaX8LyD4RVcAoXHuoim5OhvrpZezB1uUsN484f8gocSX8YTAud_4JPcRwyFjLSktwfaYmCowTOKjl_XWyrwc-hBTHlKs4EvEpD9TS8KXbHqThUdYByTwzXDzuBPOMh1HfT82zGT_5dSOEec",
                    IsFavorite = false
                },
                new ProductItem
                {
                    Id = 11,
                    Name = "Radiador Aluminio",
                    Compatibility = "Toyota Corolla 1.8L",
                    Price = 135.00m,
                    OriginalPrice = 180.00m,
                    StockStatus = "Disponible",
                    Category = "Motores",
                    Brand = "Toyota",
                    Year = "2020-2024",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBQGBq5gUDbPJg2ZzHydI217J4h46Mt1zAg9qHHR0ZjaVe9f5bsZ4BnFUdC5QtPSWkSiPrddlaugkOArxyQLxQp8zOLd_63oUCv6tiHPIxqsZTpJ_lj2Vl1KfroKWgGfUYuaD2CEe09a0RvgbYXQL6on0K_kPNzYfD9_VhQRJZtxZ_2e9cEMBQ61dIV1zZv4uPt05liZ3gvWwQoLPsEMDkE-YElHk9i5JCdUs-AQlX0krkjyOeU7oWiU0gVoz3L8EzYermoubszSWk",
                    IsFavorite = true
                },
                new ProductItem
                {
                    Id = 12,
                    Name = "Sensor de Oxígeno (O2)",
                    Compatibility = "Nissan Altima 2.5L",
                    Price = 55.00m,
                    StockStatus = "Disponible",
                    Category = "Eléctrico",
                    Brand = "Nissan",
                    Year = "2019-2023",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuACc5moWRM2H2Gn5bMEezMvvvd_OO5QJNhSsCvDJpMRpcRnJRE5DTm-bsP7GOcEMCG1-o5M-Yj9MGyj-wrShZTPQmCFjfiXR3A_EwwxbN4jh_L7ro_f6fW3BlnhOBP2OCL6dxiFnpRXPujyK1roxsd37d8Dn6NJQPFkdoax0uUQ3OTtLN-CbhmS5tc2qRzr9DlYVIsGfwhA49JMldv4exUrjzeoUPUy7bE3Hw3uUTbwQCoUj6ztsA22QT0EFSUDqpl8BzgQllfbFEg",
                    IsFavorite = false
                }
            };

            filteredProducts = new List<ProductItem>(allProducts);
            isLoading = false;
            StateHasChanged();
        }

        private void HandleSearch()
        {
            ApplyFilters();
        }

        private void FilterByCategory(string category)
        {
            selectedCategory = category;
            CloseAllDropdowns();
            ApplyFilters();
        }

        private void SelectBrand(string brand)
        {
            selectedBrand = brand == "Todas" ? null : brand;
            CloseAllDropdowns();
            ApplyFilters();
        }

        private void SelectYear(string year)
        {
            selectedYear = year == "Todos" ? null : year;
            CloseAllDropdowns();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = allProducts.AsEnumerable();

            // Filtrar por categoría
            if (selectedCategory != "Todos")
            {
                result = result.Where(p => p.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por marca
            if (!string.IsNullOrEmpty(selectedBrand))
            {
                result = result.Where(p => p.Brand.Equals(selectedBrand, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por año
            if (!string.IsNullOrEmpty(selectedYear))
            {
                result = result.Where(p => p.Year.Contains(selectedYear, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                result = result.Where(p =>
                    p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Compatibility.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Brand.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            filteredProducts = result.ToList();
            StateHasChanged();
        }

        private void ClearFilters()
        {
            searchQuery = string.Empty;
            selectedCategory = "Todos";
            selectedBrand = null;
            selectedYear = null;
            ApplyFilters();
        }

        private void ToggleFilter(int index)
        {
            // Cerrar todos los dropdowns excepto el clickeado
            for (int i = 0; i < showFilterDropdown.Length; i++)
            {
                showFilterDropdown[i] = (i == index) ? !showFilterDropdown[i] : false;
            }
            StateHasChanged();
        }

        private void CloseAllDropdowns()
        {
            for (int i = 0; i < showFilterDropdown.Length; i++)
            {
                showFilterDropdown[i] = false;
            }
        }

        private void ToggleFiltersModal()
        {
            // Implementar modal de filtros para móvil
            Console.WriteLine("Abrir modal de filtros");
        }

        // UI Helper Methods
        private string GetStockBadgeClass(string status)
        {
            return status switch
            {
                "Disponible" => "bg-green-500/90",
                "Bajo Stock" => "bg-orange-500/90",
                "Agotado" => "bg-red-500/90",
                _ => "bg-slate-500/90"
            };
        }

        // Action Handlers
        private void AddToCart(int productId)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                Console.WriteLine($"🛒 Agregado al carrito: {product.Name}");
                cartItemsCount++;
                StateHasChanged();
            }
        }

        private void ToggleFavorite(int productId)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                product.IsFavorite = !product.IsFavorite;
                Console.WriteLine($"❤️ Favorito: {product.Name} - {product.IsFavorite}");
                StateHasChanged();
            }
        }

        private void ToggleNotifications()
        {
            hasNewNotifications = false;
            StateHasChanged();
        }

        private void GoToCart()
        {
            NavigationManager?.NavigateTo("/client/cart");
        }

        private void GoBack()
        {
            NavigationManager?.NavigateTo("/client/dashboard");
        }

        // Data Model
        private class ProductItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Compatibility { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal? OriginalPrice { get; set; }
            public string StockStatus { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public string Year { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public bool IsFavorite { get; set; }
        }
    }
}

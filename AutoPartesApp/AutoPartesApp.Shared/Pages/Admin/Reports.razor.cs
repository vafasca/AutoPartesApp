using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Reports : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private string selectedPeriod = "month"; // "week", "month", "custom"
        private string selectedPeriodText = "Octubre 2023";
        private string currentMonthYear = "Octubre 2023";
        private DateTime currentMonth = new DateTime(2023, 10, 1);
        private int startDayIndex = -1;
        private int endDayIndex = -1;

        // Data
        private List<string> daysOfWeek = new() { "D", "L", "M", "M", "J", "V", "S" };
        private List<CalendarDay> calendarDays = new();
        private List<StatCard> statsCards = new();
        private List<CategoryStat> salesByCategory = new();
        private List<DeliveryStat> deliveryStats = new();
        private List<TopProduct> topProducts = new();

        // Calculated Values
        private string totalSales = "$45.2k";
        private string topCategory = "Motor";
        private int deliveryRate = 94;

        protected override void OnInitialized()
        {
            LoadData();
            GenerateCalendar();
        }

        private void LoadData()
        {
            LoadStatsCards();
            LoadSalesByCategory();
            LoadDeliveryStats();
            LoadTopProducts();
        }

        private void LoadStatsCards()
        {
            statsCards = new List<StatCard>
            {
                new StatCard
                {
                    Title = "Total Ventas",
                    Value = "$45,230",
                    Icon = "trending_up",
                    IconColor = "text-primary",
                    TrendText = "+12.5%",
                    TrendColor = "text-[#0bda5b]",
                    TrendIcon = "trending_up"
                },
                new StatCard
                {
                    Title = "Pedidos",
                    Value = "1,240",
                    Icon = "shopping_cart",
                    IconColor = "text-orange-500",
                    TrendText = "+5.2%",
                    TrendColor = "text-[#0bda5b]",
                    TrendIcon = "trending_up"
                },
                new StatCard
                {
                    Title = "Clientes Nuevos",
                    Value = "248",
                    Icon = "group_add",
                    IconColor = "text-green-500",
                    TrendText = "+18%",
                    TrendColor = "text-[#0bda5b]",
                    TrendIcon = "trending_up"
                }
            };
        }

        private void LoadSalesByCategory()
        {
            salesByCategory = new List<CategoryStat>
            {
                new CategoryStat { Name = "Motor", Percentage = 45, Color = "bg-primary" },
                new CategoryStat { Name = "Luces", Percentage = 25, Color = "bg-indigo-400" },
                new CategoryStat { Name = "Frenos", Percentage = 20, Color = "bg-slate-400" },
                new CategoryStat { Name = "Otros", Percentage = 10, Color = "bg-slate-200" }
            };
        }

        private void LoadDeliveryStats()
        {
            deliveryStats = new List<DeliveryStat>
            {
                new DeliveryStat { Label = "A tiempo", Height = 120, Color = "bg-primary" },
                new DeliveryStat { Label = "Retrasado", Height = 40, Color = "bg-slate-200 dark:bg-[#233648]" },
                new DeliveryStat { Label = "Devuelto", Height = 15, Color = "bg-slate-200 dark:bg-[#233648]" }
            };
        }

        private void LoadTopProducts()
        {
            topProducts = new List<TopProduct>
            {
                new TopProduct
                {
                    Id = 1,
                    Name = "Filtro de Aceite Bosch",
                    Category = "Motor",
                    UnitsSold = 145,
                    Revenue = 1812.50m,
                    Trend = "up",
                    TrendPercentage = 12,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAtiox9TJXUAiSseJQmSfXikwbJGHUEPi7N9wx0xwszHMjibwcpBLcCKJZoX8UU-tKGn78_MfENXELASdx92BT6uUbxcEVPH6kj3brP0-QBnBvvDx1WZD7M71arhjEu9oL-2_SMcXH8aklAdmgLQbTCimzVSgmn8SovX9aKFvV7AZqVlPVwEfKzs7U7OHgWkZV5XMGZ1_KgRRAZhDDl1llbR7IvJkQBzZUjAVPtGAgcpZ3FR_Efn-Xs_0RVXCMyXqZRBOu4tg5mWgM"
                },
                new TopProduct
                {
                    Id = 2,
                    Name = "Pastillas Freno Brembo",
                    Category = "Frenos",
                    UnitsSold = 98,
                    Revenue = 8330.00m,
                    Trend = "up",
                    TrendPercentage = 8,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCOz3tONHKIaAOG9OjRD-BhVcPZTtoGQu9tczLFBL-SaquR5SM7e3uEAzTGI8o579UNXA0CJGHzKz8BEbniV4TrjMva4JRZeS01_8ohm6zAMwkdPnanwdwNqIPcvhmHl73OGYXX_5IksWAJHNTzYiVpj5qHgfLGH4VrN1Lx07sSmrqmX5PMxjaiqY7ZSbOmHAzh1_dXKhMmydOm32lnVKBpQCagUL2TsvtgtrWMPX4eU1yjEp0UYAsX1S5lqCcnaG0qmZJQkgTtv8A"
                },
                new TopProduct
                {
                    Id = 3,
                    Name = "Bujías Iridium (4 pack)",
                    Category = "Eléctrico",
                    UnitsSold = 87,
                    Revenue = 3732.30m,
                    Trend = "down",
                    TrendPercentage = 3,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuA5ZwHq5SboWBdf1V4jV8rnureJKwvmHb_lOrx-kJ7WwiN37yHDETz8XnMZUF9mgTnjytBRanBT4sV_kgNW-yP01lVMzyldmd-Meavd8UYBl5-QKDKEqHYI9t_pLOveBdltGMyCULw3a8X6jjCFnposww9kgJyeqoX0nSIoQQ44szfFVIpqifcq33rHqu-bhX09P-YD7IQG-7MWb46tiKFhF8BJi20X_X3HCKiKBJHE2Mne6J61bnSCGF1MgD85aaWuURzmlECQlZs"
                },
                new TopProduct
                {
                    Id = 4,
                    Name = "Amortiguador Monroe",
                    Category = "Suspensión",
                    UnitsSold = 64,
                    Revenue = 7040.00m,
                    Trend = "up",
                    TrendPercentage = 15,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAkC65h6BdIXOtTFGHvyR4S-3G6YpdxTUP5Se_rVVKDrA4dEPtdN5kAOYjpFcesozvpN7g6pgi6K85dslj7Jw4F_ll_7RhFVM56FYKGoQ9333jOujlFWfzEC0YE6ud49XbausyZZxsgV32L4ecPyINnKUbpkgfnCYCHHCpzkGbn1T1xyNRTBMZH8XlZ2Gc_vxVhu6GtPrbnGW-9ES59YI1dYSPcCJgaPPmvU7SCPaRetG3az3D2O00YfQ1kgNSm4ozRKXO-9YidcFQ"
                },
                new TopProduct
                {
                    Id = 5,
                    Name = "Batería AGM 60Ah",
                    Category = "Eléctrico",
                    UnitsSold = 52,
                    Revenue = 6240.00m,
                    Trend = "up",
                    TrendPercentage = 20,
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCfeqZOOmLto5AuFqiHyCkVj5jITAfAiump3dSS40ePery4jUahRV63eS3CzHfXe6YXoqhN7gxm6L--BbiG4AoBfwu_wBTXxrZQisNeVGAaP7O4chDg2CEPqII0aK3iwC3qFtH2KXUi3s6CUyO4k-CMK7_pTfxKl3pTRbUBSZDsquT1nu5uxIMu89yQjIq_Fx4dT51mXxxQtJA3uHvyMUk_8Mt2LpLZNKBXIP-HTBXj1LFaOznpYS9ndnazVkR3m_uRMyg2z98OWMc"
                }
            };
        }

        private void GenerateCalendar()
        {
            calendarDays.Clear();

            var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Días del mes anterior (grises)
            var previousMonth = firstDayOfMonth.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            for (int i = firstDayOfWeek - 1; i >= 0; i--)
            {
                calendarDays.Add(new CalendarDay
                {
                    DayNumber = daysInPreviousMonth - i,
                    IsCurrentMonth = false
                });
            }

            // Días del mes actual
            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                calendarDays.Add(new CalendarDay
                {
                    DayNumber = day,
                    IsCurrentMonth = true
                });
            }

            // Días del siguiente mes (para completar la grilla)
            int remainingDays = 42 - calendarDays.Count; // 6 semanas completas
            for (int day = 1; day <= remainingDays; day++)
            {
                calendarDays.Add(new CalendarDay
                {
                    DayNumber = day,
                    IsCurrentMonth = false
                });
            }
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            Console.WriteLine("🔔 Toggle notifications");
        }

        private void ChangePeriod(string period)
        {
            selectedPeriod = period;

            selectedPeriodText = period switch
            {
                "week" => "Última Semana",
                "month" => "Octubre 2023",
                "custom" => "Personalizado",
                _ => "Octubre 2023"
            };

            // Resetear selección del calendario
            startDayIndex = -1;
            endDayIndex = -1;

            StateHasChanged();
            Console.WriteLine($"📅 Período cambiado a: {period}");
        }

        private void PreviousMonth()
        {
            currentMonth = currentMonth.AddMonths(-1);
            currentMonthYear = currentMonth.ToString("MMMM yyyy");
            GenerateCalendar();
            StateHasChanged();
        }

        private void NextMonth()
        {
            currentMonth = currentMonth.AddMonths(1);
            currentMonthYear = currentMonth.ToString("MMMM yyyy");
            GenerateCalendar();
            StateHasChanged();
        }

        private void SelectCalendarDay(CalendarDay day, int index)
        {
            if (!day.IsCurrentMonth) return;

            if (startDayIndex == -1)
            {
                // Primera selección
                startDayIndex = index;
                endDayIndex = -1;
            }
            else if (endDayIndex == -1)
            {
                // Segunda selección
                if (index < startDayIndex)
                {
                    // Si selecciona antes del inicio, intercambiar
                    endDayIndex = startDayIndex;
                    startDayIndex = index;
                }
                else
                {
                    endDayIndex = index;
                }
            }
            else
            {
                // Ya hay un rango, empezar de nuevo
                startDayIndex = index;
                endDayIndex = -1;
            }

            StateHasChanged();
        }

        // UI Helpers
        private string GetPeriodButtonClass(string period)
        {
            var baseClass = "text-xs lg:text-sm font-bold transition-colors";
            if (selectedPeriod == period)
            {
                return $"{baseClass} bg-primary text-white";
            }
            return $"{baseClass} bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-700";
        }

        private string GetCalendarDayClass(CalendarDay day, int index)
        {
            var baseClass = "h-10 w-full text-xs font-medium";

            if (!day.IsCurrentMonth)
            {
                return $"{baseClass} opacity-20 cursor-default";
            }

            bool isInRange = false;
            bool isStart = index == startDayIndex;
            bool isEnd = index == endDayIndex;

            if (startDayIndex != -1 && endDayIndex != -1)
            {
                isInRange = index > startDayIndex && index < endDayIndex;
            }

            if (isStart && isEnd)
            {
                return $"{baseClass} text-white bg-primary rounded-full";
            }
            else if (isStart)
            {
                return $"{baseClass} text-white bg-primary rounded-l-full";
            }
            else if (isEnd)
            {
                return $"{baseClass} text-white bg-primary rounded-r-full";
            }
            else if (isInRange)
            {
                return $"{baseClass} bg-primary/20";
            }

            return $"{baseClass} hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg cursor-pointer";
        }

        private string GetTrendClass(string trend)
        {
            return trend == "up" ? "text-[#0bda5b]" : "text-[#fa6238]";
        }

        private string GetTrendIcon(string trend)
        {
            return trend == "up" ? "trending_up" : "trending_down";
        }

        // Action Handlers
        private void ViewAllProducts()
        {
            Console.WriteLine("📦 Ver todos los productos");
            NavigationManager?.NavigateTo("/admin/inventory");
        }

        private void ExportPDF()
        {
            Console.WriteLine("📄 Exportar reporte a PDF");
            // Implementar exportación a PDF
        }

        private void ExportExcel()
        {
            Console.WriteLine("📊 Exportar reporte a Excel");
            // Implementar exportación a Excel
        }

        // Data Models
        private class StatCard
        {
            public string Title { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string IconColor { get; set; } = string.Empty;
            public string TrendText { get; set; } = string.Empty;
            public string TrendColor { get; set; } = string.Empty;
            public string TrendIcon { get; set; } = string.Empty;
        }

        private class CalendarDay
        {
            public int DayNumber { get; set; }
            public bool IsCurrentMonth { get; set; }
        }

        private class CategoryStat
        {
            public string Name { get; set; } = string.Empty;
            public int Percentage { get; set; }
            public string Color { get; set; } = string.Empty;
        }

        private class DeliveryStat
        {
            public string Label { get; set; } = string.Empty;
            public int Height { get; set; }
            public string Color { get; set; } = string.Empty;
        }

        private class TopProduct
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public int UnitsSold { get; set; }
            public decimal Revenue { get; set; }
            public string Trend { get; set; } = string.Empty; // "up" or "down"
            public int TrendPercentage { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}

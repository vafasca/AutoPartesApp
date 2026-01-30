using AutoPartesApp.Shared.Models.Admin;
using AutoPartesApp.Shared.Services.Admin;
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

        [Inject]
        private ReportService? ReportService { get; set; }

        [Inject]
        private ExportService? ExportService { get; set; }

        // ViewModel - CORREGIDO: Ahora se inicializa correctamente
        private ReportViewModel? viewModel;

        // State
        private string selectedPeriod = "month";
        private string selectedPeriodText = string.Empty;
        private string currentMonthYear = string.Empty;
        private DateTime currentMonth = DateTime.Now;
        private int startDayIndex = -1;
        private int endDayIndex = -1;
        private string activeTab = "sales";

        // Data
        private List<string> daysOfWeek = new() { "D", "L", "M", "M", "J", "V", "S" };
        private List<CalendarDay> calendarDays = new();

        protected override async Task OnInitializedAsync()
        {
            // CORREGIDO: Inicializar viewModel con ReportService
            if (ReportService != null)
            {
                viewModel = new ReportViewModel(ReportService);
            }

            await LoadInitialData();
            GenerateCalendar();
        }

        private async Task LoadInitialData()
        {
            if (viewModel == null) return;

            viewModel.IsLoading = true;
            viewModel.ErrorMessage = string.Empty;

            try
            {
                // Configurar fechas iniciales (último mes)
                var dateTo = DateTime.UtcNow;
                var dateFrom = dateTo.AddMonths(-1);

                // CORREGIDO: Usar propiedades directas en lugar de viewModel.Filters
                viewModel.DateFrom = dateFrom;
                viewModel.DateTo = dateTo;

                selectedPeriodText = $"{dateFrom:dd/MM/yyyy} - {dateTo:dd/MM/yyyy}";
                currentMonthYear = currentMonth.ToString("MMMM yyyy");

                // Cargar reporte inicial (ventas)
                await viewModel.LoadSalesReportAsync();
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = $"Error al cargar datos iniciales: {ex.Message}";
            }
            finally
            {
                viewModel.IsLoading = false;
                StateHasChanged();
            }
        }

        private void GenerateCalendar()
        {
            calendarDays.Clear();

            var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Días del mes anterior
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

            // Días del siguiente mes
            int remainingDays = 42 - calendarDays.Count;
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
            // TODO: Implementar notificaciones
        }

        private async Task ChangePeriod(string period)
        {
            if (viewModel == null) return;

            selectedPeriod = period;

            var now = DateTime.UtcNow;

            switch (period)
            {
                case "week":
                    viewModel.DateFrom = now.AddDays(-7);
                    viewModel.DateTo = now;
                    selectedPeriodText = "Última Semana";
                    break;

                case "month":
                    viewModel.DateFrom = now.AddMonths(-1);
                    viewModel.DateTo = now;
                    selectedPeriodText = "Último Mes";
                    break;

                case "custom":
                    selectedPeriodText = "Personalizado";
                    break;
            }

            // Resetear selección del calendario
            startDayIndex = -1;
            endDayIndex = -1;

            // Recargar datos del tab activo
            await ReloadCurrentTab();
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

        private async Task SelectCalendarDay(CalendarDay day, int index)
        {
            if (!day.IsCurrentMonth || viewModel == null) return;

            if (startDayIndex == -1)
            {
                startDayIndex = index;
                endDayIndex = -1;
            }
            else if (endDayIndex == -1)
            {
                if (index < startDayIndex)
                {
                    endDayIndex = startDayIndex;
                    startDayIndex = index;
                }
                else
                {
                    endDayIndex = index;
                }

                // Calcular fechas seleccionadas
                var startDay = calendarDays[startDayIndex].DayNumber;
                var endDay = calendarDays[endDayIndex].DayNumber;

                viewModel.DateFrom = new DateTime(currentMonth.Year, currentMonth.Month, startDay);
                viewModel.DateTo = new DateTime(currentMonth.Year, currentMonth.Month, endDay).AddDays(1).AddSeconds(-1);

                selectedPeriodText = $"{viewModel.DateFrom:dd/MM/yyyy} - {viewModel.DateTo:dd/MM/yyyy}";

                // Recargar datos
                await ReloadCurrentTab();
            }
            else
            {
                startDayIndex = index;
                endDayIndex = -1;
            }

            StateHasChanged();
        }

        private async Task ChangeTab(string tab)
        {
            if (activeTab == tab || viewModel == null) return;

            activeTab = tab;

            // Cargar datos del nuevo tab
            switch (tab)
            {
                case "sales":
                    if (viewModel.SalesReport == null)
                        await viewModel.LoadSalesReportAsync();
                    break;
                case "inventory":
                    if (viewModel.InventoryReport == null)
                        await viewModel.LoadInventoryReportAsync();
                    break;
                case "customers":
                    if (viewModel.CustomerReport == null)
                        await viewModel.LoadCustomerReportAsync();
                    break;
                case "deliveries":
                    if (viewModel.DeliveryReport == null)
                        await viewModel.LoadDeliveryReportAsync();
                    break;
                case "orders":
                    if (viewModel.OrderReport == null)
                        await viewModel.LoadOrderReportAsync();
                    break;
                case "financial":
                    if (viewModel.FinancialReport == null)
                        await viewModel.LoadFinancialReportAsync();
                    break;
            }

            StateHasChanged();
        }

        private async Task ReloadCurrentTab()
        {
            if (viewModel == null) return;

            switch (activeTab)
            {
                case "sales":
                    await viewModel.LoadSalesReportAsync();
                    break;
                case "inventory":
                    await viewModel.LoadInventoryReportAsync();
                    break;
                case "customers":
                    await viewModel.LoadCustomerReportAsync();
                    break;
                case "deliveries":
                    await viewModel.LoadDeliveryReportAsync();
                    break;
                case "orders":
                    await viewModel.LoadOrderReportAsync();
                    break;
                case "financial":
                    await viewModel.LoadFinancialReportAsync();
                    break;
            }
        }

        private async Task RetryLoadData()
        {
            await LoadInitialData();
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

        private string GetTabClass(string tab)
        {
            var baseClass = "flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-colors whitespace-nowrap";
            if (activeTab == tab)
            {
                return $"{baseClass} bg-primary text-white";
            }
            return $"{baseClass} bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-700";
        }

        // Export Actions
        private async Task ExportPDF()
        {
            if (viewModel == null || ExportService == null) return;

            try
            {
                viewModel.IsLoading = true;

                string title = GetReportTitle();
                string fileName = $"reporte_{activeTab}_{DateTime.Now:yyyyMMdd}.pdf";

                object? reportData = activeTab switch
                {
                    "sales" => viewModel.SalesReport,
                    "inventory" => viewModel.InventoryReport,
                    "customers" => viewModel.CustomerReport,
                    "deliveries" => viewModel.DeliveryReport,
                    "orders" => viewModel.OrderReport,
                    "financial" => viewModel.FinancialReport,
                    _ => null
                };

                if (reportData != null)
                {
                    await ExportService.ExportToPdfAsync(title, reportData, fileName);
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = $"Error al exportar PDF: {ex.Message}";
            }
            finally
            {
                viewModel.IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task ExportExcel()
        {
            if (viewModel == null || ExportService == null) return;

            try
            {
                viewModel.IsLoading = true;

                string title = GetReportTitle();
                string fileName = $"reporte_{activeTab}_{DateTime.Now:yyyyMMdd}.csv";

                var csvData = ConvertReportToCSV();

                if (csvData != null && csvData.Any())
                {
                    await ExportService.ExportToExcelAsync(title, csvData, fileName);
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = $"Error al exportar Excel: {ex.Message}";
            }
            finally
            {
                viewModel.IsLoading = false;
                StateHasChanged();
            }
        }

        // Helper Methods
        private string GetReportTitle()
        {
            return activeTab switch
            {
                "sales" => "Reporte de Ventas",
                "inventory" => "Reporte de Inventario",
                "customers" => "Reporte de Clientes",
                "deliveries" => "Reporte de Entregas",
                "orders" => "Reporte de Órdenes",
                "financial" => "Reporte Financiero",
                _ => "Reporte"
            };
        }

        private List<Dictionary<string, object>>? ConvertReportToCSV()
        {
            if (viewModel == null) return null;

            return activeTab switch
            {
                "sales" => ConvertSalesReportToCSV(),
                "inventory" => ConvertInventoryReportToCSV(),
                "customers" => ConvertCustomerReportToCSV(),
                "deliveries" => ConvertDeliveryReportToCSV(),
                "orders" => ConvertOrderReportToCSV(),
                "financial" => ConvertFinancialReportToCSV(),
                _ => null
            };
        }

        private List<Dictionary<string, object>>? ConvertSalesReportToCSV()
        {
            if (viewModel?.SalesReport == null) return null;

            var data = new List<Dictionary<string, object>>();

            // Resumen general
            data.Add(new Dictionary<string, object>
    {
        { "Métrica", "Total Ingresos" },
        { "Valor", viewModel.SalesReport.TotalRevenue },
        { "Período", $"{viewModel.DateFrom:dd/MM/yyyy} - {viewModel.DateTo:dd/MM/yyyy}" }
    });

            data.Add(new Dictionary<string, object>
    {
        { "Métrica", "Total Órdenes" },
        { "Valor", viewModel.SalesReport.TotalOrders },
        { "Período", $"{viewModel.DateFrom:dd/MM/yyyy} - {viewModel.DateTo:dd/MM/yyyy}" }
    });

            data.Add(new Dictionary<string, object>
    {
        { "Métrica", "Ticket Promedio" },
        { "Valor", viewModel.SalesReport.AverageOrderValue },
        { "Período", $"{viewModel.DateFrom:dd/MM/yyyy} - {viewModel.DateTo:dd/MM/yyyy}" }
    });

            // Top productos
            if (viewModel.SalesReport.TopProducts?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" },
            { "Período", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Producto", "Producto" },
            { "Categoría", "Categoría" },
            { "Unidades", "Unidades Vendidas" },
            { "Ingresos", "Ingresos" }
        });

                foreach (var product in viewModel.SalesReport.TopProducts)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Producto", product.Name },
                { "Categoría", product.CategoryName },
                { "Unidades", product.UnitsSold },
                { "Ingresos", product.Revenue }
            });
                }
            }

            return data;
        }

        private List<Dictionary<string, object>>? ConvertInventoryReportToCSV()
        {
            if (viewModel?.InventoryReport == null) return null;

            var data = new List<Dictionary<string, object>>
    {
        new()
        {
            { "Métrica", "Total Productos" },
            { "Valor", viewModel.InventoryReport.TotalProducts }
        },
        new()
        {
            { "Métrica", "Valor Total Inventario" },
            { "Valor", viewModel.InventoryReport.TotalValue }
        },
        new()
        {
            { "Métrica", "Productos Sin Stock" },
            { "Valor", viewModel.InventoryReport.OutOfStock }
        },
        new()
        {
            { "Métrica", "Productos Bajo Stock" },
            { "Valor", viewModel.InventoryReport.LowStock }
        }
    };

            // Valor por categoría
            if (viewModel.InventoryReport.ValueByCategory?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Categoría", "Categoría" },
            { "Valor Total", "Valor Total" },
            { "Unidades", "Total Unidades" },
            { "Porcentaje", "Porcentaje" }
        });

                foreach (var category in viewModel.InventoryReport.ValueByCategory)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Categoría", category.CategoryName },
                { "Valor Total", category.TotalValue },
                { "Unidades", category.TotalUnits },
                { "Porcentaje", $"{category.Percentage}%" }
            });
                }
            }

            return data;
        }

        private List<Dictionary<string, object>>? ConvertCustomerReportToCSV()
        {
            if (viewModel?.CustomerReport == null) return null;

            var data = new List<Dictionary<string, object>>
    {
        new()
        {
            { "Métrica", "Total Clientes" },
            { "Valor", viewModel.CustomerReport.TotalCustomers }
        },
        new()
        {
            { "Métrica", "Nuevos Clientes" },
            { "Valor", viewModel.CustomerReport.NewCustomers }
        },
        new()
        {
            { "Métrica", "Tasa de Retención" },
            { "Valor", $"{viewModel.CustomerReport.RetentionRate}%" }
        }
    };

            // Top clientes
            if (viewModel.CustomerReport.TopCustomers?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Cliente", "Nombre" },
            { "Email", "Email" },
            { "Órdenes", "Total Órdenes" },
            { "Gastado", "Total Gastado" }
        });

                foreach (var customer in viewModel.CustomerReport.TopCustomers)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Cliente", customer.FullName },
                { "Email", customer.Email },
                { "Órdenes", customer.TotalOrders },
                { "Gastado", customer.TotalSpent }
            });
                }
            }

            return data;
        }

        private List<Dictionary<string, object>>? ConvertDeliveryReportToCSV()
        {
            if (viewModel?.DeliveryReport == null) return null;

            var data = new List<Dictionary<string, object>>
    {
        new()
        {
            { "Métrica", "Total Entregas" },
            { "Valor", viewModel.DeliveryReport.TotalDeliveries }
        },
        new()
        {
            { "Métrica", "Entregas Completadas" },
            { "Valor", viewModel.DeliveryReport.CompletedDeliveries }
        },
        new()
        {
            { "Métrica", "Entregas Pendientes" },
            { "Valor", viewModel.DeliveryReport.PendingDeliveries }
        },
        new()
        {
            { "Métrica", "Tiempo Promedio" },
            { "Valor", $"{viewModel.DeliveryReport.AverageDeliveryTime} min" }
        },
        new()
        {
            { "Métrica", "Tasa de Eficiencia" },
            { "Valor", $"{viewModel.DeliveryReport.EfficiencyRate}%" }
        }
    };

            // Top drivers
            if (viewModel.DeliveryReport.TopDrivers?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Repartidor", "Nombre" },
            { "Entregas", "Total Entregas" },
            { "Completadas", "Completadas" },
            { "Tiempo Prom", "Tiempo Promedio" },
            { "Eficiencia", "Eficiencia" }
        });

                foreach (var driver in viewModel.DeliveryReport.TopDrivers)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Repartidor", driver.FullName },
                { "Entregas", driver.TotalDeliveries },
                { "Completadas", driver.CompletedDeliveries },
                { "Tiempo Prom", $"{driver.AverageTime} min" },
                { "Eficiencia", $"{driver.EfficiencyRate}%" }
            });
                }
            }

            return data;
        }

        private List<Dictionary<string, object>>? ConvertOrderReportToCSV()
        {
            if (viewModel?.OrderReport == null) return null;

            var data = new List<Dictionary<string, object>>
    {
        new()
        {
            { "Métrica", "Total Órdenes" },
            { "Valor", viewModel.OrderReport.TotalOrders }
        },
        new()
        {
            { "Métrica", "Tiempo Promedio Procesamiento" },
            { "Valor", $"{viewModel.OrderReport.AverageProcessingTime} min" }
        },
        new()
        {
            { "Métrica", "Tasa de Cancelación" },
            { "Valor", $"{viewModel.OrderReport.CancellationRate}%" }
        },
        new()
        {
            { "Métrica", "Órdenes Pendientes" },
            { "Valor", viewModel.OrderReport.PendingOrders }
        }
    };

            // Órdenes por estado
            if (viewModel.OrderReport.OrdersByStatus?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Estado", "Estado" },
            { "Cantidad", "Cantidad" },
            { "Porcentaje", "Porcentaje" }
        });

                foreach (var status in viewModel.OrderReport.OrdersByStatus)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Estado", status.Status },
                { "Cantidad", status.Count },
                { "Porcentaje", $"{status.Percentage}%" }
            });
                }
            }

            return data;
        }

        private List<Dictionary<string, object>>? ConvertFinancialReportToCSV()
        {
            if (viewModel?.FinancialReport == null) return null;

            var data = new List<Dictionary<string, object>>
    {
        new()
        {
            { "Métrica", "Ingresos Totales" },
            { "Valor", viewModel.FinancialReport.TotalRevenue }
        },
        new()
        {
            { "Métrica", "Ticket Promedio" },
            { "Valor", viewModel.FinancialReport.AverageTicket }
        }
    };

            // Ingresos por período
            if (viewModel.FinancialReport.RevenueByPeriod?.Any() == true)
            {
                data.Add(new Dictionary<string, object>
        {
            { "Métrica", "" },
            { "Valor", "" }
        });

                data.Add(new Dictionary<string, object>
        {
            { "Período", "Período" },
            { "Ingresos", "Ingresos" },
            { "Órdenes", "Cantidad Órdenes" }
        });

                foreach (var period in viewModel.FinancialReport.RevenueByPeriod)
                {
                    data.Add(new Dictionary<string, object>
            {
                { "Período", period.Period },
                { "Ingresos", period.Revenue },
                { "Órdenes", period.OrderCount }
            });
                }
            }

            return data;
        }

        // Data Model
        private class CalendarDay
        {
            public int DayNumber { get; set; }
            public bool IsCurrentMonth { get; set; }
        }
    }
}

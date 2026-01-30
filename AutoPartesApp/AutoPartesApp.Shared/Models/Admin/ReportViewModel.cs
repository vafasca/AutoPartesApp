using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Shared.Services.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly ReportService _reportService;

        // State
        private bool _isLoading;
        private string _errorMessage = string.Empty;
        private string _selectedReportType = "sales"; // sales, inventory, customers, deliveries, orders, financial

        // Filtros
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private ReportPeriodType _selectedPeriod = ReportPeriodType.Month;
        private string? _selectedCategoryId;

        // Reportes
        private SalesReportDto? _salesReport;
        private InventoryReportDto? _inventoryReport;
        private CustomerReportDto? _customerReport;
        private DeliveryReportDto? _deliveryReport;
        private OrderReportDto? _orderReport;
        private FinancialReportDto? _financialReport;

        public ReportViewModel(ReportService reportService)
        {
            _reportService = reportService;
            InitializeDefaultDates();
        }

        // Properties - State
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public string SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                _selectedReportType = value;
                OnPropertyChanged(nameof(SelectedReportType));
            }
        }

        // Properties - Filtros
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(nameof(DateFrom)); }
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(nameof(DateTo)); }
        }

        public ReportPeriodType SelectedPeriod
        {
            get => _selectedPeriod;
            set { _selectedPeriod = value; OnPropertyChanged(nameof(SelectedPeriod)); }
        }

        public string? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set { _selectedCategoryId = value; OnPropertyChanged(nameof(SelectedCategoryId)); }
        }

        // Properties - Reportes
        public SalesReportDto? SalesReport
        {
            get => _salesReport;
            set { _salesReport = value; OnPropertyChanged(nameof(SalesReport)); }
        }

        public InventoryReportDto? InventoryReport
        {
            get => _inventoryReport;
            set { _inventoryReport = value; OnPropertyChanged(nameof(InventoryReport)); }
        }

        public CustomerReportDto? CustomerReport
        {
            get => _customerReport;
            set { _customerReport = value; OnPropertyChanged(nameof(CustomerReport)); }
        }

        public DeliveryReportDto? DeliveryReport
        {
            get => _deliveryReport;
            set { _deliveryReport = value; OnPropertyChanged(nameof(DeliveryReport)); }
        }

        public OrderReportDto? OrderReport
        {
            get => _orderReport;
            set { _orderReport = value; OnPropertyChanged(nameof(OrderReport)); }
        }

        public FinancialReportDto? FinancialReport
        {
            get => _financialReport;
            set { _financialReport = value; OnPropertyChanged(nameof(FinancialReport)); }
        }

        // Methods - Inicialización
        private void InitializeDefaultDates()
        {
            DateTo = DateTime.Today;
            DateFrom = DateTime.Today.AddMonths(-1);
        }

        // Methods - Cargar Reportes
        public async Task LoadSalesReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                SalesReport = await _reportService.GetSalesReportAsync(
                    DateFrom,
                    DateTo,
                    SelectedCategoryId,
                    SelectedPeriod
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte de ventas: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadInventoryReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                InventoryReport = await _reportService.GetInventoryReportAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte de inventario: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadCustomerReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                CustomerReport = await _reportService.GetCustomerReportAsync(
                    DateFrom,
                    DateTo
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte de clientes: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadDeliveryReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                DeliveryReport = await _reportService.GetDeliveryReportAsync(
                    DateFrom,
                    DateTo
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte de entregas: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadOrderReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                OrderReport = await _reportService.GetOrderReportAsync(
                    DateFrom,
                    DateTo
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte de órdenes: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadFinancialReportAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                FinancialReport = await _reportService.GetFinancialReportAsync(
                    DateFrom,
                    DateTo
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte financiero: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadCurrentReportAsync()
        {
            switch (SelectedReportType)
            {
                case "sales":
                    await LoadSalesReportAsync();
                    break;
                case "inventory":
                    await LoadInventoryReportAsync();
                    break;
                case "customers":
                    await LoadCustomerReportAsync();
                    break;
                case "deliveries":
                    await LoadDeliveryReportAsync();
                    break;
                case "orders":
                    await LoadOrderReportAsync();
                    break;
                case "financial":
                    await LoadFinancialReportAsync();
                    break;
            }
        }

        // Methods - Cambiar Período
        public async Task ChangePeriodAsync(ReportPeriodType period)
        {
            SelectedPeriod = period;

            // Ajustar fechas según período seleccionado
            DateTo = DateTime.Today;
            DateFrom = period switch
            {
                ReportPeriodType.Day => DateTo.Value.AddDays(-7),
                ReportPeriodType.Week => DateTo.Value.AddMonths(-1),
                ReportPeriodType.Month => DateTo.Value.AddMonths(-6),
                ReportPeriodType.Year => DateTo.Value.AddYears(-2),
                _ => DateTo.Value.AddMonths(-1)
            };

            await LoadCurrentReportAsync();
        }

        public async Task SetCustomDateRangeAsync(DateTime from, DateTime to)
        {
            DateFrom = from;
            DateTo = to;
            SelectedPeriod = ReportPeriodType.Custom;
            await LoadCurrentReportAsync();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

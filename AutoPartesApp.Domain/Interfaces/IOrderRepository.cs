using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IOrderRepository
    {
        // ========== MÉTODOS EXISTENTES (CRUD BÁSICO) ==========
        Task<Order?> GetByIdAsync(string id);
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Obtener órdenes en un rango de fechas con filtros opcionales
        /// </summary>
        Task<List<Order>> GetOrdersByDateRangeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            OrderStatus? status = null,
            string? userId = null);

        /// <summary>
        /// Obtener estadísticas de órdenes por período
        /// </summary>
        Task<(int TotalOrders, decimal TotalRevenue, decimal AverageOrderValue)> GetOrderStatsByPeriodAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener productos más vendidos en un período
        /// </summary>
        Task<List<(string ProductId, string ProductName, string CategoryName, int UnitsSold, decimal Revenue, string ImageUrl)>>
            GetTopSellingProductsAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10);

        /// <summary>
        /// Obtener ventas agrupadas por categoría
        /// </summary>
        Task<List<(string CategoryId, string CategoryName, decimal TotalRevenue, int TotalUnits)>>
            GetSalesByCategoryAsync(
                DateTime dateFrom,
                DateTime dateTo);

        /// <summary>
        /// Obtener ingresos agrupados por período (día/semana/mes/año)
        /// </summary>
        Task<List<(DateTime Period, decimal Revenue, int OrderCount)>>
            GetRevenueByPeriodAsync(
                DateTime dateFrom,
                DateTime dateTo,
                string periodType = "day"); // "day", "week", "month", "year"

        /// <summary>
        /// Obtener órdenes agrupadas por estado
        /// </summary>
        Task<List<(OrderStatus Status, int Count)>> GetOrdersByStatusAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null);

        /// <summary>
        /// Obtener valor promedio de orden
        /// </summary>
        Task<decimal> GetAverageOrderValueAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener tasa de cancelación de órdenes
        /// </summary>
        Task<decimal> GetCancellationRateAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener tiempo promedio de procesamiento de órdenes
        /// </summary>
        Task<TimeSpan> GetAverageProcessingTimeAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener órdenes pendientes
        /// </summary>
        Task<List<Order>> GetPendingOrdersAsync();

        /// <summary>
        /// Obtener estadísticas de comparación entre dos períodos
        /// </summary>
        Task<(decimal CurrentRevenue, decimal PreviousRevenue, decimal GrowthPercentage)>
            GetPeriodComparisonAsync(
                DateTime currentFrom,
                DateTime currentTo,
                DateTime previousFrom,
                DateTime previousTo);
    }
}

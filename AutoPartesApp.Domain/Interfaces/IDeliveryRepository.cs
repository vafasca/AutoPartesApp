using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    /// <summary>
    /// Repositorio para gestión de entregas
    /// </summary>
    public interface IDeliveryRepository
    {
        /// <summary>
        /// Obtener entrega por ID
        /// </summary>
        Task<Delivery?> GetByIdAsync(string id);

        /// <summary>
        /// Obtener todas las entregas
        /// </summary>
        Task<List<Delivery>> GetAllAsync();

        /// <summary>
        /// Obtener entregas por repartidor
        /// </summary>
        Task<List<Delivery>> GetByDriverIdAsync(string driverId);

        /// <summary>
        /// Obtener entrega por ID de orden
        /// </summary>
        Task<Delivery?> GetByOrderIdAsync(string orderId);

        /// <summary>
        /// Crear nueva entrega
        /// </summary>
        Task<Delivery> CreateAsync(Delivery delivery);

        /// <summary>
        /// Actualizar entrega
        /// </summary>
        Task<Delivery> UpdateAsync(Delivery delivery);

        /// <summary>
        /// Eliminar entrega
        /// </summary>
        Task<bool> DeleteAsync(string id);

        // ========== MÉTODOS PARA REPORTES DE ENTREGAS ==========

        /// <summary>
        /// Obtener entregas en un rango de fechas
        /// </summary>
        Task<List<Delivery>> GetDeliveriesByDateRangeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            DeliveryStatus? status = null);

        /// <summary>
        /// Obtener estadísticas de entregas por repartidor
        /// </summary>
        Task<List<(string DriverId, string DriverName, int TotalDeliveries, int CompletedDeliveries, TimeSpan AverageTime, decimal EfficiencyRate)>>
            GetDeliveryStatsByDriverAsync(
                DateTime dateFrom,
                DateTime dateTo);

        /// <summary>
        /// Obtener tiempo promedio de entrega
        /// </summary>
        Task<TimeSpan> GetAverageDeliveryTimeAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string? driverId = null);

        /// <summary>
        /// Obtener tasa de eficiencia de entregas (completadas vs totales)
        /// </summary>
        Task<decimal> GetDeliveryEfficiencyAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener entregas agrupadas por zona geográfica
        /// </summary>
        Task<List<(string City, string State, int TotalDeliveries, TimeSpan AverageTime)>>
            GetDeliveriesByZoneAsync(
                DateTime dateFrom,
                DateTime dateTo);

        /// <summary>
        /// Obtener entregas por estado
        /// </summary>
        Task<List<(DeliveryStatus Status, int Count)>> GetDeliveriesByStatusAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null);

        /// <summary>
        /// Obtener entregas pendientes (sin asignar o en proceso)
        /// </summary>
        Task<List<Delivery>> GetPendingDeliveriesAsync();

        /// <summary>
        /// Obtener entregas retrasadas (exceden tiempo estimado)
        /// </summary>
        Task<List<Delivery>> GetDelayedDeliveriesAsync();

        /// <summary>
        /// Obtener top repartidores por eficiencia
        /// </summary>
        Task<List<(string DriverId, string DriverName, int CompletedDeliveries, decimal EfficiencyRate, TimeSpan AverageTime)>>
            GetTopDriversAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10);

        /// <summary>
        /// Obtener estadísticas globales de entregas
        /// </summary>
        Task<(int TotalDeliveries, int Completed, int Pending, int InProgress, int Cancelled, decimal SuccessRate)>
            GetDeliveryStatsAsync(
                DateTime dateFrom,
                DateTime dateTo);

        /// <summary>
        /// Obtener entregas con sus órdenes completas (Include)
        /// </summary>
        Task<List<Delivery>> GetDeliveriesWithOrdersAsync(
            DateTime dateFrom,
            DateTime dateTo,
            string? driverId = null);
    }
}

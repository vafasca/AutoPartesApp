using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetTopCustomersUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public GetTopCustomersUseCase(
            IUserRepository userRepository,
            IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<TopCustomerDto>> ExecuteAsync(
            ReportFilterDto filter,
            int topN = 10)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-6); // Últimos 6 meses

            // Obtener clientes
            var allUsers = await _userRepository.GetAllAsync();
            var clients = allUsers.Where(u => u.RoleType == RoleType.Client).ToList();

            // Obtener órdenes del período
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(dateFrom, dateTo);

            // Calcular estadísticas por cliente
            var topCustomers = clients
                .Select(c =>
                {
                    var customerOrders = orders.Where(o => o.UserId == c.Id).ToList();
                    var totalSpent = customerOrders.Sum(o => o.Total.Amount);

                    return new TopCustomerDto
                    {
                        UserId = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        TotalOrders = customerOrders.Count,
                        TotalSpent = totalSpent,
                        LastOrderDate = customerOrders.Any()
                            ? customerOrders.Max(o => o.CreatedAt)
                            : (DateTime?)null,
                        AvatarUrl = c.AvatarUrl
                    };
                })
                .Where(c => c.TotalSpent > 0) // Solo clientes con compras
                .OrderByDescending(c => c.TotalSpent)
                .Take(topN)
                .ToList();

            return topCustomers;
        }
    }
}

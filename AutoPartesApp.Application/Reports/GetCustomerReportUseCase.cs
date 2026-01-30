using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetCustomerReportUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public GetCustomerReportUseCase(
            IUserRepository userRepository,
            IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        public async Task<CustomerReportDto> ExecuteAsync(ReportFilterDto filter)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-1);

            // Obtener todos los clientes
            var allUsers = await _userRepository.GetAllAsync();
            var clients = allUsers.Where(u => u.RoleType == RoleType.Client).ToList();

            // Clientes nuevos en el período
            var newCustomers = clients
                .Where(c => c.CreatedAt >= dateFrom && c.CreatedAt <= dateTo)
                .Count();

            // Clientes inactivos (sin órdenes en los últimos 90 días)
            var inactiveThreshold = DateTime.UtcNow.AddDays(-90);
            var allOrders = await _orderRepository.GetAllAsync();

            var activeClientIds = allOrders
                .Where(o => o.CreatedAt >= inactiveThreshold)
                .Select(o => o.UserId)
                .Distinct()
                .ToHashSet();

            var inactiveCustomers = clients
                .Where(c => !activeClientIds.Contains(c.Id))
                .Count();

            // Top clientes
            var topCustomers = await GetTopCustomers(clients, allOrders, 10);

            // Distribución geográfica
            var geographicDistribution = GetGeographicDistribution(clients);

            // Tasa de retención (clientes activos)
            var activeCustomers = clients.Count - inactiveCustomers;
            var retentionRate = clients.Count > 0
                ? ((decimal)activeCustomers / clients.Count) * 100
                : 0;

            return new CustomerReportDto
            {
                TotalCustomers = clients.Count,
                NewCustomers = newCustomers,
                TopCustomers = topCustomers,
                InactiveCustomers = inactiveCustomers,
                GeographicDistribution = geographicDistribution,
                RetentionRate = retentionRate,
                ActiveCustomers = activeCustomers
            };
        }

        private async Task<List<TopCustomerDto>> GetTopCustomers(
            List<Domain.Entities.User> clients,
            List<Domain.Entities.Order> allOrders,
            int topN)
        {
            var customerStats = clients
                .Select(c =>
                {
                    var customerOrders = allOrders.Where(o => o.UserId == c.Id).ToList();
                    return new TopCustomerDto
                    {
                        UserId = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        TotalOrders = customerOrders.Count,
                        TotalSpent = customerOrders.Sum(o => o.Total.Amount),
                        LastOrderDate = customerOrders.Any()
                            ? customerOrders.Max(o => o.CreatedAt)
                            : (DateTime?)null,
                        AvatarUrl = c.AvatarUrl
                    };
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(topN)
                .ToList();

            return customerStats;
        }

        private List<GeographicDistributionDto> GetGeographicDistribution(
            List<Domain.Entities.User> clients)
        {
            var distribution = clients
                .Where(c => !string.IsNullOrEmpty(c.AddressCity))
                .GroupBy(c => new
                {
                    City = c.AddressCity ?? "N/A",
                    State = c.AddressState ?? "N/A",
                    Country = c.AddressCountry ?? "N/A"
                })
                .Select(g => new
                {
                    g.Key.City,
                    g.Key.State,
                    g.Key.Country,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var total = distribution.Sum(d => d.Count);

            return distribution.Select(d => new GeographicDistributionDto
            {
                City = d.City,
                State = d.State,
                Country = d.Country,
                TotalCustomers = d.Count,
                Percentage = total > 0 ? ((decimal)d.Count / total) * 100 : 0
            }).ToList();
        }
    }
}

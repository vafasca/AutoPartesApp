using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Users
{
    public class GetUserByIdUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDetailDto?> ExecuteAsync(string userId)
        {
            // Obtener usuario básico primero
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            // Crear DTO base
            var userDetail = new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role,
                RoleType = user.RoleType,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                AddressStreet = user.AddressStreet,
                AddressCity = user.AddressCity,
                AddressState = user.AddressState,
                AddressCountry = user.AddressCountry,
                AddressZipCode = user.AddressZipCode,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };

            // Cargar información específica según el rol
            if (user.RoleType == RoleType.Client)
            {
                var userWithOrders = await _userRepository.GetUserWithOrdersAsync(userId);
                if (userWithOrders != null)
                {
                    userDetail.OrdersSummary = BuildOrderSummary(userWithOrders);
                }
            }
            else if (user.RoleType == RoleType.Delivery)
            {
                var userWithDeliveries = await _userRepository.GetUserWithDeliveriesAsync(userId);
                if (userWithDeliveries != null)
                {
                    userDetail.DeliveriesSummary = BuildDeliverySummary(userWithDeliveries);
                }
            }

            return userDetail;
        }

        private UserOrderSummaryDto BuildOrderSummary(User user)
        {
            var orders = user.Orders ?? new List<Order>();

            return new UserOrderSummaryDto
            {
                TotalOrders = orders.Count,
                CompletedOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing),
                CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
                TotalSpent = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.Total?.Amount ?? 0),
                Currency = "USD",
                LastOrderDate = orders.Any() ? orders.Max(o => o.CreatedAt) : null,
                FirstOrderDate = orders.Any() ? orders.Min(o => o.CreatedAt) : null,
                RecentOrders = orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .Select(o => new RecentOrderDto
                    {
                        OrderId = o.Id,
                        OrderNumber = o.OrderNumber,
                        CreatedAt = o.CreatedAt,
                        Status = o.Status.ToString(),
                        Total = o.Total?.Amount ?? 0,
                        Currency = o.Total?.Currency ?? "USD"
                    })
                    .ToList()
            };
        }

        private UserDeliverySummaryDto BuildDeliverySummary(User user)
        {
            var deliveries = user.Orders?
                .Where(o => o.Delivery != null)
                .Select(o => o.Delivery!)
                .ToList() ?? new List<Delivery>();

            return new UserDeliverySummaryDto
            {
                TotalDeliveries = deliveries.Count,
                CompletedDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.Delivered),
                InProgressDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.OnRoute || d.Status == DeliveryStatus.PickedUp || d.Status == DeliveryStatus.Assigned),
                CancelledDeliveries = deliveries.Count(d => d.Status == DeliveryStatus.Failed),
                LastDeliveryDate = deliveries.Any() ? deliveries.Max(d => d.DeliveredAt) : null,
                FirstDeliveryDate = deliveries.Any() ? deliveries.Min(d => d.AssignedAt) : null,
                RecentDeliveries = deliveries
                    .OrderByDescending(d => d.AssignedAt)
                    .Take(5)
                    .Select(d => new RecentDeliveryDto
                    {
                        DeliveryId = d.Id,
                        OrderNumber = d.Order?.OrderNumber ?? "N/A",
                        AssignedAt = d.AssignedAt,
                        DeliveredAt = d.DeliveredAt,
                        Status = d.Status.ToString()
                    })
                    .ToList()
            };
        }
    }
}

using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(string id);
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(string id);
    }
}

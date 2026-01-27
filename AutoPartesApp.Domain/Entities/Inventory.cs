using System;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesApp.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public virtual Product Product { get; set; }
        
        public int StockQuantity { get; set; }
        
        public int MinimumStock { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
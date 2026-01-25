using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; } // Material icon name
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navegación
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

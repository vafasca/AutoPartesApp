using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; } // Material icon name
        public bool IsActive { get; set; } = true;

        // Navegación
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

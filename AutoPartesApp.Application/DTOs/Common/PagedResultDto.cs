using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.Common
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Propiedades calculadas
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;
        public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

        // Constructor vacío
        public PagedResultDto()
        {
        }

        // Constructor con parámetros
        public PagedResultDto(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}

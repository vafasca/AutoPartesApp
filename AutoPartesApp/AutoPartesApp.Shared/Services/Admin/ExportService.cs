using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class ExportService
    {
        private readonly IJSRuntime _jsRuntime;

        public ExportService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Exporta datos a PDF usando jsPDF (debe estar cargado en el cliente)
        /// </summary>
        public async Task ExportToPdfAsync(string title, object data, string filename = "reporte.pdf")
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                await _jsRuntime.InvokeVoidAsync("exportToPdf", title, json, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar PDF: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Exporta datos a Excel/CSV usando descarga directa
        /// </summary>
        public async Task ExportToExcelAsync(string title, List<Dictionary<string, object>> data, string filename = "reporte.csv")
        {
            try
            {
                var csv = ConvertToCSV(data);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var base64 = Convert.ToBase64String(bytes);

                await _jsRuntime.InvokeVoidAsync("downloadFile", filename, "text/csv", base64);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar Excel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Convierte una lista de diccionarios a formato CSV
        /// </summary>
        private string ConvertToCSV(List<Dictionary<string, object>> data)
        {
            if (data == null || !data.Any())
                return string.Empty;

            var sb = new StringBuilder();

            // Headers
            var headers = data.First().Keys;
            sb.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            // Rows
            foreach (var row in data)
            {
                var values = headers.Select(h =>
                {
                    var value = row.ContainsKey(h) ? row[h]?.ToString() ?? "" : "";
                    return $"\"{value.Replace("\"", "\"\"")}\""; // Escape quotes
                });
                sb.AppendLine(string.Join(",", values));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exporta tabla HTML a Excel usando SheetJS (si está disponible)
        /// </summary>
        public async Task ExportTableToExcelAsync(string tableId, string filename = "reporte.xlsx")
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("exportTableToExcel", tableId, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar tabla a Excel: {ex.Message}");
                throw;
            }
        }
    }
}

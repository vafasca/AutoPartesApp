using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Core.Application.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    // [Authorize(Roles = "Admin")]
    [Route("api/v1/admin/reports")]
    [ApiController]
    [Produces("application/json")]
    public class ReportsController : ControllerBase
    {
        private readonly GetSalesReportUseCase _getSalesReport;
        private readonly GetSalesByCategoryUseCase _getSalesByCategory;
        private readonly GetTopSellingProductsUseCase _getTopProducts;
        private readonly GetInventoryReportUseCase _getInventoryReport;
        private readonly GetLowRotationProductsUseCase _getLowRotationProducts;
        private readonly GetCustomerReportUseCase _getCustomerReport;
        private readonly GetTopCustomersUseCase _getTopCustomers;
        private readonly GetDeliveryReportUseCase _getDeliveryReport;
        private readonly GetTopDriversUseCase _getTopDrivers;
        private readonly GetOrderReportUseCase _getOrderReport;
        private readonly GetFinancialReportUseCase _getFinancialReport;
        private readonly GetRevenueByPeriodUseCase _getRevenueByPeriod;

        public ReportsController(
            GetSalesReportUseCase getSalesReport,
            GetSalesByCategoryUseCase getSalesByCategory,
            GetTopSellingProductsUseCase getTopProducts,
            GetInventoryReportUseCase getInventoryReport,
            GetLowRotationProductsUseCase getLowRotationProducts,
            GetCustomerReportUseCase getCustomerReport,
            GetTopCustomersUseCase getTopCustomers,
            GetDeliveryReportUseCase getDeliveryReport,
            GetTopDriversUseCase getTopDrivers,
            GetOrderReportUseCase getOrderReport,
            GetFinancialReportUseCase getFinancialReport,
            GetRevenueByPeriodUseCase getRevenueByPeriod)
        {
            _getSalesReport = getSalesReport;
            _getSalesByCategory = getSalesByCategory;
            _getTopProducts = getTopProducts;
            _getInventoryReport = getInventoryReport;
            _getLowRotationProducts = getLowRotationProducts;
            _getCustomerReport = getCustomerReport;
            _getTopCustomers = getTopCustomers;
            _getDeliveryReport = getDeliveryReport;
            _getTopDrivers = getTopDrivers;
            _getOrderReport = getOrderReport;
            _getFinancialReport = getFinancialReport;
            _getRevenueByPeriod = getRevenueByPeriod;
        }

        /// <summary>
        /// Obtiene reporte completo de ventas con totales, promedios y gráficos
        /// </summary>
        /// <param name="dateFrom">Fecha inicio (opcional)</param>
        /// <param name="dateTo">Fecha fin (opcional)</param>
        /// <param name="categoryId">ID de categoría para filtrar (opcional)</param>
        /// <param name="period">Período de agrupación: Day, Week, Month, Year (opcional)</param>
        /// <returns>Reporte de ventas completo</returns>
        [HttpGet("sales")]
        [ProducesResponseType(typeof(SalesReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SalesReportDto>> GetSalesReport(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string? categoryId = null,
            [FromQuery] ReportPeriodType? period = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Period = period
                };

                var report = await _getSalesReport.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte de ventas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ventas agrupadas por categoría
        /// </summary>
        [HttpGet("sales-by-category")]
        [ProducesResponseType(typeof(List<SalesByCategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SalesByCategoryDto>>> GetSalesByCategory(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getSalesByCategory.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ventas por categoría", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los N productos más vendidos
        /// </summary>
        /// <param name="top">Cantidad de productos a retornar (default: 10)</param>
        /// <param name="dateFrom">Fecha inicio (opcional)</param>
        /// <param name="dateTo">Fecha fin (opcional)</param>
        [HttpGet("top-products")]
        [ProducesResponseType(typeof(List<TopProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopProductDto>>> GetTopProducts(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getTopProducts.ExecuteAsync(filter, top);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top productos", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte completo de inventario
        /// </summary>
        [HttpGet("inventory")]
        [ProducesResponseType(typeof(InventoryReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
        {
            try
            {
                var report = await _getInventoryReport.ExecuteAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte de inventario", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene productos con baja rotación
        /// </summary>
        /// <param name="dateFrom">Fecha inicio para análisis (opcional)</param>
        /// <param name="dateTo">Fecha fin para análisis (opcional)</param>
        /// <param name="threshold">Umbral de unidades vendidas (default: 5)</param>
        [HttpGet("low-rotation-products")]
        [ProducesResponseType(typeof(List<TopProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopProductDto>>> GetLowRotationProducts(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int threshold = 5)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getLowRotationProducts.ExecuteAsync(filter, threshold);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos de baja rotación", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte completo de clientes
        /// </summary>
        [HttpGet("customers")]
        [ProducesResponseType(typeof(CustomerReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerReportDto>> GetCustomerReport(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getCustomerReport.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte de clientes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los N mejores clientes
        /// </summary>
        /// <param name="top">Cantidad de clientes a retornar (default: 10)</param>
        /// <param name="dateFrom">Fecha inicio (opcional)</param>
        /// <param name="dateTo">Fecha fin (opcional)</param>
        [HttpGet("top-customers")]
        [ProducesResponseType(typeof(List<TopCustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopCustomerDto>>> GetTopCustomers(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getTopCustomers.ExecuteAsync(filter, top);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top clientes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte completo de entregas
        /// </summary>
        [HttpGet("deliveries")]
        [ProducesResponseType(typeof(DeliveryReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DeliveryReportDto>> GetDeliveryReport(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getDeliveryReport.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte de entregas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los N mejores repartidores
        /// </summary>
        /// <param name="top">Cantidad de repartidores a retornar (default: 10)</param>
        /// <param name="dateFrom">Fecha inicio (opcional)</param>
        /// <param name="dateTo">Fecha fin (opcional)</param>
        [HttpGet("top-drivers")]
        [ProducesResponseType(typeof(List<TopDriverDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TopDriverDto>>> GetTopDrivers(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getTopDrivers.ExecuteAsync(filter, top);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top repartidores", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte completo de órdenes
        /// </summary>
        [HttpGet("orders")]
        [ProducesResponseType(typeof(OrderReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderReportDto>> GetOrderReport(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getOrderReport.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte de órdenes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene reporte financiero completo
        /// </summary>
        [HttpGet("financial")]
        [ProducesResponseType(typeof(FinancialReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<FinancialReportDto>> GetFinancialReport(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var report = await _getFinancialReport.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener reporte financiero", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ingresos agrupados por período
        /// </summary>
        /// <param name="dateFrom">Fecha inicio</param>
        /// <param name="dateTo">Fecha fin</param>
        /// <param name="period">Período: Day, Week, Month, Year (default: Month)</param>
        [HttpGet("revenue-by-period")]
        [ProducesResponseType(typeof(List<PeriodRevenueDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PeriodRevenueDto>>> GetRevenueByPeriod(
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] ReportPeriodType period = ReportPeriodType.Month) // ✅ CORREGIDO: Usar enum
        {
            try
            {
                var filter = new ReportFilterDto
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Period = period
                };

                var report = await _getRevenueByPeriod.ExecuteAsync(filter);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ingresos por período", error = ex.Message });
            }
        }
    }
}

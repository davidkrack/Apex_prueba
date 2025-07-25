using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceManagement.Api.Data;

namespace InvoiceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(InvoiceDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Reporte: Facturas consistentes, con más de 30 días vencidas sin pago o nota de crédito
        /// </summary>
        /// <returns>Lista de facturas vencidas</returns>
        [HttpGet("overdue-invoices")]
        public async Task<ActionResult<object>> GetOverdueInvoicesReport()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-30);

                // Primero traemos las facturas con Include
                var allInvoices = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.CreditNotes)
                    .Include(i => i.InvoicePayment)
                    .Where(i => i.IsConsistent && 
                               i.PaymentDueDate < cutoffDate && 
                               i.PaymentStatus != "Paid")
                    .ToListAsync();

                // Luego filtramos en memoria las que no tienen notas de crédito
                var overdueInvoices = allInvoices
                    .Where(i => !i.CreditNotes.Any())
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.PaymentDueDate,
                        i.TotalAmount,
                        i.PaymentStatus,
                        DaysOverdue = (int)(DateTime.Now - i.PaymentDueDate).TotalDays,
                        Customer = new
                        {
                            i.Customer.CustomerName,
                            i.Customer.CustomerEmail,
                            i.Customer.CustomerRun
                        }
                    })
                    .OrderByDescending(i => i.DaysOverdue)
                    .ToList();

                var totalAmount = overdueInvoices.Sum(i => i.TotalAmount);
                var averageDaysOverdue = overdueInvoices.Any() 
                    ? overdueInvoices.Average(i => i.DaysOverdue) 
                    : 0;

                return Ok(new
                {
                    reportName = "Facturas Vencidas (Más de 30 días)",
                    generatedDate = DateTime.Now,
                    criteria = "Facturas consistentes, vencidas hace más de 30 días, sin pago ni notas de crédito",
                    summary = new
                    {
                        totalInvoices = overdueInvoices.Count,
                        totalAmount,
                        averageDaysOverdue = Math.Round(averageDaysOverdue, 1)
                    },
                    invoices = overdueInvoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de facturas vencidas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Reporte: Resumen total y porcentaje de facturas por estado de pago
        /// </summary>
        /// <returns>Estadísticas de estados de pago</returns>
        [HttpGet("payment-status-summary")]
        public async Task<ActionResult<object>> GetPaymentStatusSummary()
        {
            try
            {
                // Solo facturas consistentes para el reporte
                var paymentStats = await _context.Invoices
                    .Where(i => i.IsConsistent)
                    .GroupBy(i => i.PaymentStatus)
                    .Select(g => new
                    {
                        PaymentStatus = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(i => i.TotalAmount),
                        AverageAmount = g.Average(i => i.TotalAmount)
                    })
                    .ToListAsync();

                var totalInvoices = paymentStats.Sum(ps => ps.Count);
                var grandTotal = paymentStats.Sum(ps => ps.TotalAmount);

                var detailedStats = paymentStats.Select(ps => new
                {
                    ps.PaymentStatus,
                    ps.Count,
                    ps.TotalAmount,
                    ps.AverageAmount,
                    Percentage = totalInvoices > 0 ? Math.Round((double)ps.Count / totalInvoices * 100, 2) : 0,
                    AmountPercentage = grandTotal > 0 ? Math.Round((double)ps.TotalAmount / (double)grandTotal * 100, 2) : 0
                }).OrderByDescending(ps => ps.Count).ToList();

                // Obtener facturas próximas a vencer (próximos 7 días)
                var upcomingDue = await _context.Invoices
                    .Where(i => i.IsConsistent && 
                               i.PaymentStatus == "Pending" && 
                               i.PaymentDueDate <= DateTime.Now.AddDays(7) &&
                               i.PaymentDueDate >= DateTime.Now)
                    .CountAsync();

                return Ok(new
                {
                    reportName = "Resumen de Estados de Pago",
                    generatedDate = DateTime.Now,
                    summary = new
                    {
                        totalConsistentInvoices = totalInvoices,
                        grandTotalAmount = grandTotal,
                        upcomingDueInvoices = upcomingDue
                    },
                    paymentStatusBreakdown = detailedStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar resumen de estados de pago");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Reporte: Facturas inconsistentes (total declarado no coincide con suma de productos)
        /// </summary>
        /// <returns>Lista de facturas inconsistentes</returns>
        [HttpGet("inconsistent-invoices")]
        public async Task<ActionResult<object>> GetInconsistentInvoicesReport()
        {
            try
            {
                // Traemos todas las facturas inconsistentes con sus detalles
                var allInconsistentInvoices = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                    .Where(i => !i.IsConsistent)
                    .ToListAsync();

                // Procesamos en memoria para calcular diferencias
                var inconsistentInvoices = allInconsistentInvoices
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.TotalAmount,
                        CalculatedTotal = i.InvoiceDetails.Sum(d => d.Subtotal),
                        Difference = i.TotalAmount - i.InvoiceDetails.Sum(d => d.Subtotal),
                        Customer = new
                        {
                            i.Customer.CustomerName,
                            i.Customer.CustomerEmail,
                            i.Customer.CustomerRun
                        },
                        ProductCount = i.InvoiceDetails.Count
                    })
                    .OrderByDescending(i => Math.Abs(i.Difference))
                    .ToList();

                var totalDifference = inconsistentInvoices.Sum(i => Math.Abs(i.Difference));
                var averageDifference = inconsistentInvoices.Any() 
                    ? inconsistentInvoices.Average(i => Math.Abs(i.Difference)) 
                    : 0;

                return Ok(new
                {
                    reportName = "Facturas Inconsistentes",
                    generatedDate = DateTime.Now,
                    criteria = "Facturas donde el total declarado no coincide con la suma de productos",
                    summary = new
                    {
                        totalInconsistentInvoices = inconsistentInvoices.Count,
                        totalAbsoluteDifference = totalDifference,
                        averageAbsoluteDifference = Math.Round(averageDifference, 2)
                    },
                    invoices = inconsistentInvoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de facturas inconsistentes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Reporte consolidado con todos los indicadores principales
        /// </summary>
        /// <returns>Dashboard ejecutivo</returns>
        [HttpGet("executive-dashboard")]
        public async Task<ActionResult<object>> GetExecutiveDashboard()
        {
            try
            {
                // Estadísticas generales
                var totalInvoices = await _context.Invoices.CountAsync();
                var consistentInvoices = await _context.Invoices.CountAsync(i => i.IsConsistent);
                var inconsistentInvoices = totalInvoices - consistentInvoices;

                // Montos totales (manejo de casos sin datos)
                var totalAmount = await _context.Invoices.Where(i => i.IsConsistent).SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
                var totalCreditNotes = await _context.CreditNotes.SumAsync(cn => (decimal?)cn.CreditNoteAmount) ?? 0;
                var netAmount = totalAmount - totalCreditNotes;

                // Estados de pago
                var paidInvoices = await _context.Invoices.CountAsync(i => i.IsConsistent && i.PaymentStatus == "Paid");
                var pendingInvoices = await _context.Invoices.CountAsync(i => i.IsConsistent && i.PaymentStatus == "Pending");
                var overdueInvoices = await _context.Invoices.CountAsync(i => i.IsConsistent && i.PaymentStatus == "Overdue");

                // Facturas críticas (más de 30 días vencidas sin pago ni NC)
                var cutoffDate = DateTime.Now.AddDays(-30);
                var criticalInvoices = await _context.Invoices
                    .Include(i => i.CreditNotes)
                    .Where(i => i.IsConsistent && 
                               i.PaymentDueDate < cutoffDate && 
                               i.PaymentStatus != "Paid" && 
                               i.CreditNotes.Count == 0)
                    .CountAsync();

                // Top 5 clientes por monto (consulta simplificada para SQLite)
                var customersWithTotals = await _context.Invoices
                    .Where(i => i.IsConsistent)
                    .Include(i => i.Customer)
                    .GroupBy(i => new { i.Customer.CustomerName, i.Customer.CustomerRun })
                    .Select(g => new
                    {
                        CustomerName = g.Key.CustomerName,
                        CustomerRun = g.Key.CustomerRun,
                        TotalAmount = g.Sum(i => i.TotalAmount),
                        InvoiceCount = g.Count()
                    })
                    .ToListAsync(); // Primero traemos los datos

                // Luego ordenamos y tomamos top 5 en memoria (compatible con SQLite)
                var topCustomers = customersWithTotals
                    .OrderByDescending(c => c.TotalAmount)
                    .Take(5)
                    .ToList();

                return Ok(new
                {
                    reportName = "Dashboard Ejecutivo",
                    generatedDate = DateTime.Now,
                    overview = new
                    {
                        totalInvoices,
                        consistentInvoices,
                        inconsistentInvoices,
                        consistencyRate = totalInvoices > 0 ? Math.Round((double)consistentInvoices / totalInvoices * 100, 1) : 0
                    },
                    financials = new
                    {
                        totalAmount,
                        totalCreditNotes,
                        netAmount,
                        averageInvoiceAmount = consistentInvoices > 0 ? Math.Round(totalAmount / consistentInvoices, 2) : 0
                    },
                    paymentStatus = new
                    {
                        paid = new { count = paidInvoices, percentage = consistentInvoices > 0 ? Math.Round((double)paidInvoices / consistentInvoices * 100, 1) : 0 },
                        pending = new { count = pendingInvoices, percentage = consistentInvoices > 0 ? Math.Round((double)pendingInvoices / consistentInvoices * 100, 1) : 0 },
                        overdue = new { count = overdueInvoices, percentage = consistentInvoices > 0 ? Math.Round((double)overdueInvoices / consistentInvoices * 100, 1) : 0 }
                    },
                    alerts = new
                    {
                        criticalOverdueInvoices = criticalInvoices,
                        inconsistentInvoices
                    },
                    topCustomers = topCustomers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar dashboard ejecutivo");
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}
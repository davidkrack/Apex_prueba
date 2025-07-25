using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceManagement.Api.Data;
using InvoiceManagement.Api.Models;

namespace InvoiceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(InvoiceDbContext context, ILogger<InvoicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las facturas con filtros opcionales
        /// </summary>
        /// <param name="invoiceNumber">Número de factura (opcional)</param>
        /// <param name="invoiceStatus">Estado de factura: issued, partial, canceled (opcional)</param>
        /// <param name="paymentStatus">Estado de pago: Pending, Overdue, Paid (opcional)</param>
        /// <param name="consistentOnly">Solo facturas consistentes (opcional, por defecto true)</param>
        /// <returns>Lista de facturas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetInvoices(
            [FromQuery] int? invoiceNumber = null,
            [FromQuery] string? invoiceStatus = null,
            [FromQuery] string? paymentStatus = null,
            [FromQuery] bool consistentOnly = true)
        {
            try
            {
                var query = _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.CreditNotes)
                    .Include(i => i.InvoicePayment)
                    .AsQueryable();

                // Filtro por consistencia (por defecto solo facturas consistentes)
                if (consistentOnly)
                {
                    query = query.Where(i => i.IsConsistent);
                }

                // Filtro por número de factura
                if (invoiceNumber.HasValue)
                {
                    query = query.Where(i => i.InvoiceNumber == invoiceNumber.Value);
                }

                // Filtro por estado de factura
                if (!string.IsNullOrEmpty(invoiceStatus))
                {
                    query = query.Where(i => i.InvoiceStatus.ToLower() == invoiceStatus.ToLower());
                }

                // Filtro por estado de pago
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    query = query.Where(i => i.PaymentStatus.ToLower() == paymentStatus.ToLower());
                }

                var invoices = await query.Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.InvoiceStatus,
                    i.TotalAmount,
                    i.PaymentDueDate,
                    i.PaymentStatus,
                    i.IsConsistent,
                    Customer = new
                    {
                        i.Customer.CustomerName,
                        i.Customer.CustomerEmail,
                        i.Customer.CustomerRun
                    },
                    InvoiceDetails = i.InvoiceDetails.Select(d => new
                    {
                        d.ProductName,
                        d.UnitPrice,
                        d.Quantity,
                        d.Subtotal
                    }),
                    CreditNotes = i.CreditNotes.Select(cn => new
                    {
                        cn.CreditNoteNumber,
                        cn.CreditNoteDate,
                        cn.CreditNoteAmount
                    }),
                    Payment = i.InvoicePayment != null ? new
                    {
                        i.InvoicePayment.PaymentMethod,
                        i.InvoicePayment.PaymentDate
                    } : null,
                    TotalCreditNotes = i.CreditNotes.Sum(cn => cn.CreditNoteAmount),
                    PendingAmount = i.TotalAmount - i.CreditNotes.Sum(cn => cn.CreditNoteAmount)
                }).ToListAsync();

                return Ok(new
                {
                    data = invoices,
                    count = invoices.Count,
                    filters = new
                    {
                        invoiceNumber,
                        invoiceStatus,
                        paymentStatus,
                        consistentOnly
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facturas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una factura específica por ID
        /// </summary>
        /// <param name="id">ID de la factura</param>
        /// <returns>Detalles de la factura</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.CreditNotes)
                    .Include(i => i.InvoicePayment)
                    .Where(i => i.Id == id)
                    .Select(i => new
                    {
                        i.Id,
                        i.InvoiceNumber,
                        i.InvoiceDate,
                        i.InvoiceStatus,
                        i.TotalAmount,
                        i.DaysToDue,
                        i.PaymentDueDate,
                        i.PaymentStatus,
                        i.IsConsistent,
                        Customer = new
                        {
                            i.Customer.Id,
                            i.Customer.CustomerName,
                            i.Customer.CustomerEmail,
                            i.Customer.CustomerRun
                        },
                        InvoiceDetails = i.InvoiceDetails.Select(d => new
                        {
                            d.Id,
                            d.ProductName,
                            d.UnitPrice,
                            d.Quantity,
                            d.Subtotal
                        }),
                        CreditNotes = i.CreditNotes.Select(cn => new
                        {
                            cn.Id,
                            cn.CreditNoteNumber,
                            cn.CreditNoteDate,
                            cn.CreditNoteAmount
                        }),
                        Payment = i.InvoicePayment != null ? new
                        {
                            i.InvoicePayment.Id,
                            i.InvoicePayment.PaymentMethod,
                            i.InvoicePayment.PaymentDate
                        } : null,
                        TotalCreditNotes = i.CreditNotes.Sum(cn => cn.CreditNoteAmount),
                        PendingAmount = i.TotalAmount - i.CreditNotes.Sum(cn => cn.CreditNoteAmount)
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound(new { message = $"Factura con ID {id} no encontrada" });
                }

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura con ID {id}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas generales de facturas
        /// </summary>
        /// <returns>Resumen estadístico</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            try
            {
                var totalInvoices = await _context.Invoices.CountAsync();
                var consistentInvoices = await _context.Invoices.CountAsync(i => i.IsConsistent);
                var inconsistentInvoices = totalInvoices - consistentInvoices;

                var paymentStats = await _context.Invoices
                    .Where(i => i.IsConsistent)
                    .GroupBy(i => i.PaymentStatus)
                    .Select(g => new
                    {
                        PaymentStatus = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(i => i.TotalAmount)
                    })
                    .ToListAsync();

                var invoiceStatusStats = await _context.Invoices
                    .Where(i => i.IsConsistent)
                    .GroupBy(i => i.InvoiceStatus)
                    .Select(g => new
                    {
                        InvoiceStatus = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(i => i.TotalAmount)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    summary = new
                    {
                        totalInvoices,
                        consistentInvoices,
                        inconsistentInvoices,
                        consistencyPercentage = totalInvoices > 0 ? (double)consistentInvoices / totalInvoices * 100 : 0
                    },
                    paymentStatusBreakdown = paymentStats,
                    invoiceStatusBreakdown = invoiceStatusStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
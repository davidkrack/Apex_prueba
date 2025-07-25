using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceManagement.Api.Data;
using InvoiceManagement.Api.Models;

namespace InvoiceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditNotesController : ControllerBase
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<CreditNotesController> _logger;

        public CreditNotesController(InvoiceDbContext context, ILogger<CreditNotesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notas de crédito
        /// </summary>
        /// <returns>Lista de notas de crédito</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCreditNotes()
        {
            try
            {
                var creditNotes = await _context.CreditNotes
                    .Include(cn => cn.Invoice)
                    .ThenInclude(i => i.Customer)
                    .Select(cn => new
                    {
                        cn.Id,
                        cn.CreditNoteNumber,
                        cn.CreditNoteDate,
                        cn.CreditNoteAmount,
                        Invoice = new
                        {
                            cn.Invoice.Id,
                            cn.Invoice.InvoiceNumber,
                            cn.Invoice.TotalAmount,
                            Customer = new
                            {
                                cn.Invoice.Customer.CustomerName,
                                cn.Invoice.Customer.CustomerRun
                            }
                        }
                    })
                    .ToListAsync();

                return Ok(new { data = creditNotes, count = creditNotes.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notas de crédito");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las notas de crédito de una factura específica
        /// </summary>
        /// <param name="invoiceId">ID de la factura</param>
        /// <returns>Notas de crédito de la factura</returns>
        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetCreditNotesByInvoice(int invoiceId)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(invoiceId);
                if (invoice == null)
                {
                    return NotFound(new { message = $"Factura con ID {invoiceId} no encontrada" });
                }

                var creditNotes = await _context.CreditNotes
                    .Where(cn => cn.InvoiceId == invoiceId)
                    .Select(cn => new
                    {
                        cn.Id,
                        cn.CreditNoteNumber,
                        cn.CreditNoteDate,
                        cn.CreditNoteAmount
                    })
                    .ToListAsync();

                var totalCreditNotes = creditNotes.Sum(cn => cn.CreditNoteAmount);
                var pendingAmount = invoice.TotalAmount - totalCreditNotes;

                return Ok(new
                {
                    invoiceId,
                    invoiceNumber = invoice.InvoiceNumber,
                    invoiceTotalAmount = invoice.TotalAmount,
                    totalCreditNotes,
                    pendingAmount,
                    creditNotes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notas de crédito para factura {invoiceId}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva nota de crédito para una factura
        /// </summary>
        /// <param name="request">Datos de la nota de crédito</param>
        /// <returns>Nota de crédito creada</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateCreditNote([FromBody] CreateCreditNoteRequest request)
        {
            try
            {
                // Validar datos de entrada
                if (request.InvoiceId <= 0)
                {
                    return BadRequest(new { message = "ID de factura inválido" });
                }

                if (request.CreditNoteAmount <= 0)
                {
                    return BadRequest(new { message = "El monto de la nota de crédito debe ser mayor a cero" });
                }

                // Verificar que la factura existe
                var invoice = await _context.Invoices
                    .Include(i => i.CreditNotes)
                    .FirstOrDefaultAsync(i => i.Id == request.InvoiceId);

                if (invoice == null)
                {
                    return NotFound(new { message = $"Factura con ID {request.InvoiceId} no encontrada" });
                }

                // Validar que la factura sea consistente
                if (!invoice.IsConsistent)
                {
                    return BadRequest(new { message = "No se pueden agregar notas de crédito a facturas inconsistentes" });
                }

                // Calcular monto pendiente
                var totalExistingCreditNotes = invoice.CreditNotes.Sum(cn => cn.CreditNoteAmount);
                var pendingAmount = invoice.TotalAmount - totalExistingCreditNotes;

                // Validar que el monto no supere el saldo pendiente
                if (request.CreditNoteAmount > pendingAmount)
                {
                    return BadRequest(new
                    {
                        message = "El monto de la nota de crédito no puede superar el saldo pendiente de la factura",
                        pendingAmount,
                        requestedAmount = request.CreditNoteAmount
                    });
                }

                // Generar número de nota de crédito automático
                var maxCreditNoteNumber = await _context.CreditNotes
                    .MaxAsync(cn => (int?)cn.CreditNoteNumber) ?? 0;

                // Crear la nota de crédito
                var creditNote = new CreditNote
                {
                    CreditNoteNumber = maxCreditNoteNumber + 1,
                    CreditNoteDate = DateTime.Now,
                    CreditNoteAmount = request.CreditNoteAmount,
                    InvoiceId = request.InvoiceId
                };

                _context.CreditNotes.Add(creditNote);

                // Recalcular el estado de la factura
                var newTotalCreditNotes = totalExistingCreditNotes + request.CreditNoteAmount;
                if (newTotalCreditNotes >= invoice.TotalAmount)
                {
                    invoice.InvoiceStatus = "canceled";
                }
                else if (newTotalCreditNotes > 0)
                {
                    invoice.InvoiceStatus = "partial";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Nota de crédito {creditNote.CreditNoteNumber} creada para factura {invoice.InvoiceNumber}");

                return CreatedAtAction(
                    nameof(GetCreditNotesByInvoice),
                    new { invoiceId = request.InvoiceId },
                    new
                    {
                        creditNote.Id,
                        creditNote.CreditNoteNumber,
                        creditNote.CreditNoteDate,
                        creditNote.CreditNoteAmount,
                        invoiceId = request.InvoiceId,
                        newInvoiceStatus = invoice.InvoiceStatus,
                        newPendingAmount = invoice.TotalAmount - newTotalCreditNotes
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nota de crédito");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una nota de crédito (solo para pruebas)
        /// </summary>
        /// <param name="id">ID de la nota de crédito</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCreditNote(int id)
        {
            try
            {
                var creditNote = await _context.CreditNotes
                    .Include(cn => cn.Invoice)
                    .ThenInclude(i => i.CreditNotes)
                    .FirstOrDefaultAsync(cn => cn.Id == id);

                if (creditNote == null)
                {
                    return NotFound(new { message = $"Nota de crédito con ID {id} no encontrada" });
                }

                var invoice = creditNote.Invoice;
                _context.CreditNotes.Remove(creditNote);

                // Recalcular el estado de la factura
                var remainingCreditNotes = invoice.CreditNotes.Where(cn => cn.Id != id).Sum(cn => cn.CreditNoteAmount);
                if (remainingCreditNotes == 0)
                {
                    invoice.InvoiceStatus = "issued";
                }
                else if (remainingCreditNotes < invoice.TotalAmount)
                {
                    invoice.InvoiceStatus = "partial";
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Nota de crédito {creditNote.CreditNoteNumber} eliminada exitosamente",
                    newInvoiceStatus = invoice.InvoiceStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar nota de crédito {id}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    public class CreateCreditNoteRequest
    {
        public int InvoiceId { get; set; }
        public decimal CreditNoteAmount { get; set; }
    }
}
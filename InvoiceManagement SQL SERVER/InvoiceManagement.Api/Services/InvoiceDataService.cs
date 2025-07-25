using System.Text.Json;
using InvoiceManagement.Api.Data;
using InvoiceManagement.Api.Models;
using InvoiceManagement.Api.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagement.Api.Services
{
    public class InvoiceDataService : IInvoiceDataService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<InvoiceDataService> _logger;

        public InvoiceDataService(InvoiceDbContext context, ILogger<InvoiceDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> LoadInvoicesFromJsonAsync(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    _logger.LogError($"El archivo JSON no existe: {jsonFilePath}");
                    return false;
                }

                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                return await LoadInvoicesFromJsonStringAsync(jsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el archivo JSON");
                return false;
            }
        }

        public async Task<bool> LoadInvoicesFromJsonStringAsync(string jsonContent)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var invoiceRoot = JsonSerializer.Deserialize<InvoiceRootDto>(jsonContent, options);
                
                if (invoiceRoot?.Invoices == null || !invoiceRoot.Invoices.Any())
                {
                    _logger.LogWarning("No se encontraron facturas en el JSON");
                    return false;
                }

                _logger.LogInformation($"Procesando {invoiceRoot.Invoices.Count} facturas del JSON");

                foreach (var invoiceDto in invoiceRoot.Invoices)
                {
                    await ProcessInvoiceAsync(invoiceDto);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Datos cargados exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el JSON");
                return false;
            }
        }

        private async Task ProcessInvoiceAsync(InvoiceJsonDto invoiceDto)
        {
            try
            {
                // Verificar si la factura ya existe
                var existingInvoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceDto.InvoiceNumber);

                if (existingInvoice != null)
                {
                    _logger.LogWarning($"La factura {invoiceDto.InvoiceNumber} ya existe, se omite");
                    return;
                }

                // Procesar o crear cliente
                var customer = await GetOrCreateCustomerAsync(invoiceDto.Customer);

                // Validar consistencia de la factura
                var isConsistent = ValidateInvoiceConsistency(invoiceDto);

                // Crear la factura
                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceDto.InvoiceNumber,
                    InvoiceDate = DateTime.Parse(invoiceDto.InvoiceDate),
                    TotalAmount = invoiceDto.TotalAmount,
                    DaysToDue = invoiceDto.DaysToDue,
                    PaymentDueDate = DateTime.Parse(invoiceDto.PaymentDueDate),
                    CustomerId = customer.Id,
                    IsConsistent = isConsistent
                };

                // Calcular estado de la factura y pago
                CalculateInvoiceStatus(invoice, invoiceDto);
                CalculatePaymentStatus(invoice, invoiceDto);

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID

                // Agregar detalles de la factura
                foreach (var detailDto in invoiceDto.InvoiceDetail)
                {
                    var detail = new InvoiceDetail
                    {
                        ProductName = detailDto.ProductName,
                        UnitPrice = detailDto.UnitPrice,
                        Quantity = detailDto.Quantity,
                        Subtotal = detailDto.Subtotal,
                        InvoiceId = invoice.Id
                    };
                    _context.InvoiceDetails.Add(detail);
                }

                // Agregar información de pago si existe
                if (!string.IsNullOrEmpty(invoiceDto.InvoicePayment.PaymentMethod))
                {
                    var payment = new InvoicePayment
                    {
                        PaymentMethod = invoiceDto.InvoicePayment.PaymentMethod,
                        PaymentDate = !string.IsNullOrEmpty(invoiceDto.InvoicePayment.PaymentDate) 
                            ? DateTime.Parse(invoiceDto.InvoicePayment.PaymentDate) 
                            : null,
                        InvoiceId = invoice.Id
                    };
                    _context.InvoicePayments.Add(payment);
                }

                // Agregar notas de crédito si existen
                foreach (var creditNoteDto in invoiceDto.InvoiceCreditNote)
                {
                    var creditNote = new CreditNote
                    {
                        CreditNoteNumber = creditNoteDto.CreditNoteNumber,
                        CreditNoteDate = DateTime.Parse(creditNoteDto.CreditNoteDate),
                        CreditNoteAmount = creditNoteDto.CreditNoteAmount,
                        InvoiceId = invoice.Id
                    };
                    _context.CreditNotes.Add(creditNote);
                }

                _logger.LogInformation($"Factura {invoiceDto.InvoiceNumber} procesada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar la factura {invoiceDto.InvoiceNumber}");
            }
        }

        private async Task<Customer> GetOrCreateCustomerAsync(CustomerJsonDto customerDto)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerRun == customerDto.CustomerRun);

            if (existingCustomer != null)
            {
                return existingCustomer;
            }

            var newCustomer = new Customer
            {
                CustomerRun = customerDto.CustomerRun,
                CustomerName = customerDto.CustomerName,
                CustomerEmail = customerDto.CustomerEmail
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            return newCustomer;
        }

        private bool ValidateInvoiceConsistency(InvoiceJsonDto invoiceDto)
        {
            var calculatedTotal = invoiceDto.InvoiceDetail.Sum(d => d.Subtotal);
            return Math.Abs(calculatedTotal - invoiceDto.TotalAmount) < 0.01m; // Tolerance for decimal precision
        }

        private void CalculateInvoiceStatus(Invoice invoice, InvoiceJsonDto invoiceDto)
        {
            var totalCreditNotes = invoiceDto.InvoiceCreditNote.Sum(cn => cn.CreditNoteAmount);

            if (totalCreditNotes == 0)
            {
                invoice.InvoiceStatus = "issued";
            }
            else if (totalCreditNotes >= invoice.TotalAmount)
            {
                invoice.InvoiceStatus = "canceled";
            }
            else
            {
                invoice.InvoiceStatus = "partial";
            }
        }

        private void CalculatePaymentStatus(Invoice invoice, InvoiceJsonDto invoiceDto)
        {
            var hasPayment = !string.IsNullOrEmpty(invoiceDto.InvoicePayment.PaymentMethod);

            if (hasPayment)
            {
                invoice.PaymentStatus = "Paid";
            }
            else if (DateTime.Now > invoice.PaymentDueDate)
            {
                invoice.PaymentStatus = "Overdue";
            }
            else
            {
                invoice.PaymentStatus = "Pending";
            }
        }
    }
}
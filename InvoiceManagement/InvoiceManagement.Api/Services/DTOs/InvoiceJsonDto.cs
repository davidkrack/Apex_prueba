using System.Text.Json.Serialization;

namespace InvoiceManagement.Api.Services.DTOs
{
    public class InvoiceRootDto
    {
        [JsonPropertyName("invoices")]
        public List<InvoiceJsonDto> Invoices { get; set; } = new();
    }

    public class InvoiceJsonDto
    {
        [JsonPropertyName("invoice_number")]
        public int InvoiceNumber { get; set; }

        [JsonPropertyName("invoice_date")]
        public string InvoiceDate { get; set; } = string.Empty;

        [JsonPropertyName("invoice_status")]
        public string InvoiceStatus { get; set; } = string.Empty;

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("days_to_due")]
        public int DaysToDue { get; set; }

        [JsonPropertyName("payment_due_date")]
        public string PaymentDueDate { get; set; } = string.Empty;

        [JsonPropertyName("payment_status")]
        public string PaymentStatus { get; set; } = string.Empty;

        [JsonPropertyName("invoice_detail")]
        public List<InvoiceDetailJsonDto> InvoiceDetail { get; set; } = new();

        [JsonPropertyName("invoice_payment")]
        public InvoicePaymentJsonDto InvoicePayment { get; set; } = new();

        [JsonPropertyName("invoice_credit_note")]
        public List<CreditNoteJsonDto> InvoiceCreditNote { get; set; } = new();

        [JsonPropertyName("customer")]
        public CustomerJsonDto Customer { get; set; } = new();
    }

    public class InvoiceDetailJsonDto
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }
    }

    public class InvoicePaymentJsonDto
    {
        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_date")]
        public string? PaymentDate { get; set; }
    }

    public class CreditNoteJsonDto
    {
        [JsonPropertyName("credit_note_number")]
        public int CreditNoteNumber { get; set; }

        [JsonPropertyName("credit_note_date")]
        public string CreditNoteDate { get; set; } = string.Empty;

        [JsonPropertyName("credit_note_amount")]
        public decimal CreditNoteAmount { get; set; }
    }

    public class CustomerJsonDto
    {
        [JsonPropertyName("customer_run")]
        public string CustomerRun { get; set; } = string.Empty;

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
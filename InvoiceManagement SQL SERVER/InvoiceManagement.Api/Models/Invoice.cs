using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagement.Api.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InvoiceNumber { get; set; }
        
        [Required]
        public DateTime InvoiceDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string InvoiceStatus { get; set; } = string.Empty; // issued, partial, canceled
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public int DaysToDue { get; set; }
        
        [Required]
        public DateTime PaymentDueDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = string.Empty; // Pending, Overdue, Paid
        
        // Indica si la factura es consistente (suma de productos = total)
        public bool IsConsistent { get; set; } = true;
        
        // Foreign Key
        public int CustomerId { get; set; }
        
        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
        
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
        public virtual ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();
        public virtual InvoicePayment? InvoicePayment { get; set; }
    }
}
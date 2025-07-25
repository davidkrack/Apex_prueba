using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagement.Api.Models
{
    public class InvoicePayment
    {
        [Key]
        public int Id { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        // Foreign Key (relación 1:1 con Invoice)
        public int InvoiceId { get; set; }
        
        // Navigation Property
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;
    }
}
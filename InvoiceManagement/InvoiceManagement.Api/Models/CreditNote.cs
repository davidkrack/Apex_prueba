using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagement.Api.Models
{
    public class CreditNote
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CreditNoteNumber { get; set; }
        
        [Required]
        public DateTime CreditNoteDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditNoteAmount { get; set; }
        
        // Foreign Key
        public int InvoiceId { get; set; }
        
        // Navigation Property
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;
    }
}
using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.Api.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CustomerRun { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(250)]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        
        // Relación con las facturas
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
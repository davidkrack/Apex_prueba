using Microsoft.EntityFrameworkCore;
using InvoiceManagement.Api.Models;

namespace InvoiceManagement.Api.Data
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
        {
        }

        // DbSets para cada entidad
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de índices únicos
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerRun)
                .IsUnique();

            // Configuración de relación 1:1 entre Invoice y InvoicePayment
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.InvoicePayment)
                .WithOne(p => p.Invoice)
                .HasForeignKey<InvoicePayment>(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relación 1:N entre Invoice y InvoiceDetail
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.InvoiceDetails)
                .WithOne(d => d.Invoice)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relación 1:N entre Invoice y CreditNote
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.CreditNotes)
                .WithOne(c => c.Invoice)
                .HasForeignKey(c => c.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relación 1:N entre Customer y Invoice
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Invoices)
                .WithOne(i => i.Customer)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
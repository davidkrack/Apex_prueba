namespace InvoiceManagement.Api.Services
{
    public interface IInvoiceDataService
    {
        Task<bool> LoadInvoicesFromJsonAsync(string jsonFilePath);
        Task<bool> LoadInvoicesFromJsonStringAsync(string jsonContent);
    }
}
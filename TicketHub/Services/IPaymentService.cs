namespace TicketHub.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string currency, string paymentMethod);
        string GenerateTransactionId();
    }
}

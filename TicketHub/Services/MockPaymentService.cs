namespace TicketHub.Services
{
    public class MockPaymentService : IPaymentService
    {
        public string GenerateTransactionId()
        {
            return "TXN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string currency, string paymentMethod)
        {
            // Simulate network delay
            await Task.Delay(1000);

            // Mock success logic: Fail if amount is 0 or negative
            if (amount <= 0) return false;

            return true;
        }
    }
}

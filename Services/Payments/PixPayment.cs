public class PixPayment : IPaymentProcessor
{
    public string Method => "pix";

    public Task ProcessPayment(decimal amount, int customerId)
    {
        return Task.CompletedTask;
    }
}
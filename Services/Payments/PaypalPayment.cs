public class PaypalPayment : IPaymentProcessor
{
    public string Method => "paypal";

    public Task ProcessPayment(decimal amount, int customerId)
    {
        return Task.CompletedTask;
    }
}
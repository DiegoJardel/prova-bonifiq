public class CreditCardPayment : IPaymentProcessor
{
    public string Method => "creditcard";

    public Task ProcessPayment(decimal amount, int customerId)
    {
        return Task.CompletedTask;
    }
}

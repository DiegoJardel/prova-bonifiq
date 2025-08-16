public interface IPaymentProcessor
{
    Task ProcessPayment(decimal amount, int customerId);
    string Method { get; }
}

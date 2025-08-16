using ProvaPub.Models;
using ProvaPub.Repository;


public class OrderService
{
    private readonly TestDbContext _ctx;
    private readonly IEnumerable<IPaymentProcessor> _paymentProcessors;

    public OrderService(TestDbContext ctx, IEnumerable<IPaymentProcessor> paymentProcessors)
    {
        _ctx = ctx;
        _paymentProcessors = paymentProcessors;
    }

    public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
    {
        var processor = _paymentProcessors.FirstOrDefault(p => p.Method == paymentMethod.ToLower());
        if (processor == null)
            throw new InvalidOperationException("Método de pagamento não suportado.");

        await processor.ProcessPayment(paymentValue, customerId);

        var order = new Order
        {
            CustomerId = customerId,
            Value = paymentValue,
            OrderDate = DateTime.UtcNow 
        };

        _ctx.Orders.Add(order);
        await _ctx.SaveChangesAsync();

        return order;
    }
}

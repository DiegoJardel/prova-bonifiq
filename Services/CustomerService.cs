using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class CustomerService
    {
        private readonly TestDbContext _ctx;
        private readonly IDateTimeProvider _dateTimeProvider;
        private const int PageSize = 10;

        public CustomerService(TestDbContext ctx, IDateTimeProvider dateTimeProvider)
        {
            _ctx = ctx;
            _dateTimeProvider = dateTimeProvider;
        }

        private PagedList<T> GetPaged<T>(IQueryable<T> query, int page)
        {
            var totalCount = query.Count();
            var items = query.Skip((page - 1) * PageSize)
                             .Take(PageSize)
                             .ToList();

            return new PagedList<T>
            {
                Items = items,
                TotalCount = totalCount,
                HasNext = page * PageSize < totalCount
            };
        }

        public PagedList<Customer> ListCustomers(int page)
        {
            return GetPaged(_ctx.Customers.Include(c => c.Orders), page);
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            var baseDate = _dateTimeProvider.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0) return false;

            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100) return false;

            if (_dateTimeProvider.UtcNow.Hour < 8 || _dateTimeProvider.UtcNow.Hour > 18 ||
                _dateTimeProvider.UtcNow.DayOfWeek == DayOfWeek.Saturday || _dateTimeProvider.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}

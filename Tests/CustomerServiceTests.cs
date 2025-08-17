using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using Xunit;
namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private TestDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }

        private CustomerService CreateService(TestDbContext ctx, DateTime now)
        {
            var mockDateProvider = new Mock<IDateTimeProvider>();
            mockDateProvider.Setup(x => x.UtcNow).Returns(now);

            return new CustomerService(ctx, mockDateProvider.Object);
        }

        [Fact]
        public async Task Should_Throw_When_CustomerId_Invalid()
        {
            using var ctx = GetDbContext();
            var svc = CreateService(ctx, DateTime.UtcNow);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => svc.CanPurchase(0, 10));
        }

        [Fact]
        public async Task Should_Throw_When_PurchaseValue_Invalid()
        {
            using var ctx = GetDbContext();
            var svc = CreateService(ctx, DateTime.UtcNow);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => svc.CanPurchase(1, 0));
        }

        [Fact]
        public async Task Should_Throw_When_Customer_Not_Found()
        {
            using var ctx = GetDbContext();
            var svc = CreateService(ctx, DateTime.UtcNow);

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CanPurchase(999, 50));
        }

        [Fact]
        public async Task Should_Return_False_When_Has_Order_In_Last_Month()
        {
            using var ctx = GetDbContext();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            ctx.Orders.Add(new Order { Id = 1, CustomerId = 1, OrderDate = DateTime.UtcNow });
            ctx.SaveChanges();

            var svc = CreateService(ctx, DateTime.UtcNow);

            var result = await svc.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task Should_Return_False_When_FirstPurchase_And_Value_Greater_Than_100()
        {
            using var ctx = GetDbContext();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            ctx.SaveChanges();

            var svc = CreateService(ctx, DateTime.UtcNow);

            var result = await svc.CanPurchase(1, 200);

            Assert.False(result);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(19)]
        public async Task Should_Return_False_When_Outside_WorkingHours(int hour)
        {
            using var ctx = GetDbContext();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            ctx.SaveChanges();

            var svc = CreateService(ctx, new DateTime(2025, 6, 10, hour, 0, 0, DateTimeKind.Utc));

            var result = await svc.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Theory]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public async Task Should_Return_False_When_Weekend(DayOfWeek day)
        {
            using var ctx = GetDbContext();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            ctx.SaveChanges();

            var date = new DateTime(2025, 6, 14, 10, 0, 0, DateTimeKind.Utc);
            while (date.DayOfWeek != day) date = date.AddDays(1);

            var svc = CreateService(ctx, date);

            var result = await svc.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task Should_Return_True_When_All_Conditions_Are_Met()
        {
            using var ctx = GetDbContext();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            ctx.SaveChanges();

            var svc = CreateService(ctx, new DateTime(2025, 6, 10, 10, 0, 0, DateTimeKind.Utc));

            var result = await svc.CanPurchase(1, 50);

            Assert.True(result);
        }
    }
}

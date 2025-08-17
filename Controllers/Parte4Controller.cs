using Microsoft.AspNetCore.Mvc;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parte4Controller : ControllerBase
    {
        private readonly TestDbContext _ctx;

        public Parte4Controller(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("CanPurchase")]
        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            var svc = new CustomerService(_ctx, new SystemDateTimeProvider());
            return await svc.CanPurchase(customerId, purchaseValue);
        }
    }
}

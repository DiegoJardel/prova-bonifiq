using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class RandomService
    {
        private readonly TestDbContext _ctx;
        private readonly Random _random;

        public RandomService()
        {
            var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(@"Server=(localdb)\\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                .Options;

            _ctx = new TestDbContext(contextOptions);
            _random = new Random();
        }

        public async Task<int> GetRandom()
        {
            while (true)
            {
                var number = _random.Next(int.MaxValue);
                if (!await _ctx.Numbers.AnyAsync(x => x.Number == number))
                {
                    _ctx.Numbers.Add(new RandomNumber { Number = number });
                    await _ctx.SaveChangesAsync();
                    return number;
                }
            }
        }
    }
}

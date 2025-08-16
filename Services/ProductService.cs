using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService
    {
        private readonly TestDbContext _ctx;
        private const int PageSize = 10;

        public ProductService(TestDbContext ctx)
        {
            _ctx = ctx;
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

        public PagedList<Product> ListProducts(int page)
        {
            return GetPaged(_ctx.Products.AsQueryable(), page);
        }
    }
}

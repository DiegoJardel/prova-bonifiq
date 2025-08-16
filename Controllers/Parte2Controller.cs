using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Parte2Controller : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;

        public Parte2Controller(ProductService productService, CustomerService customerService)
        {
            _productService = productService;
            _customerService = customerService;
        }

        [HttpGet("products")]
        public PagedList<Product> ListProducts(int page)
        {
            return _productService.ListProducts(page);
        }

        [HttpGet("customers")]
        public PagedList<Customer> ListCustomers(int page)
        {
            return _customerService.ListCustomers(page);
        }
    }
}

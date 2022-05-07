using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProductShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }
    }
}

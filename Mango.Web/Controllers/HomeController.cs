using Mango.Web.Extenstions;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Service.IService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService productService;
        private readonly IShoppingCartService shoppingCartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            this.productService = productService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var getAllProductsResponse = await productService.GetAllAsync();

            IList<ProductDto>? products = new List<ProductDto>(0);

            if (getAllProductsResponse?.Result != null && getAllProductsResponse.IsSuccess)
            {
                products = getAllProductsResponse.Result.ToList();
            }
            else
            {
                TempData["error"] = getAllProductsResponse?.Message;
            }
            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var getProductByIdAsync = await productService.GetByIdAsync(id);

            ProductDto? product = null;

            if (getProductByIdAsync?.Result != null && getProductByIdAsync.IsSuccess)
            {
                product = getProductByIdAsync.Result;
            }
            else
            {
                TempData["error"] = getProductByIdAsync?.Message;
            }
            return View(product);
        }

        [HttpPost()]
        [Authorize]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto product)
        {
            var userId = User.GetId();

            var shoppingCartResponse = await shoppingCartService.GetCart(userId);


            if (shoppingCartResponse.IsSuccess)
            {

                var existedDetail = shoppingCartResponse.Result.Details.SingleOrDefault(x => x.ProductId == product.Id);

                if (existedDetail != null)
                {
                    existedDetail.Count += product.Count;
                }
                else
                {
                    shoppingCartResponse.Result.Details.Add(new CartDetailsDto
                    {
                        ProductId = product.Id,
                        Count = product.Count
                    });
                }

                var upsertResponse = await shoppingCartService.Upsert(shoppingCartResponse.Result);

                if (upsertResponse.IsSuccess)
                {
                    TempData["success"] = "Product has been added to the cart successfully.";
                }
                else 
                {
                    TempData["error"] = upsertResponse.Message;
                }                
            }
            else
            {
                TempData["error"] = "Error intializing shopping cart.";
            }

            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

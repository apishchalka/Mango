using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
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

        [Authorize]
        public IActionResult ProductCreate()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            ModelState.Remove(nameof(model.Count));

            if (ModelState.IsValid)
            {
                var createResponse = await productService.CreateAsync(model);

                if (createResponse?.Result != null && createResponse.IsSuccess)
                {
                    TempData["success"] = "Product has been created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = createResponse?.Message;
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var deleteResponse = await productService.DeleteAsync(productId);

            if (!deleteResponse.IsSuccess)
            {
                TempData["error"] = deleteResponse?.Message;
            }
            else {
                TempData["success"] = "Product deleted successfully";
            }

            return RedirectToAction(nameof(ProductIndex));
        }
    }
}

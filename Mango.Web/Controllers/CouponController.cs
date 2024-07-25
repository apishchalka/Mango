using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ICouponService couponService;

        public CouponController(ICouponService couponService)
        {
            this.couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            var couponListResponse = await couponService.GetAllAsync();

            IList<CouponDto>? couponDtos = new List<CouponDto>(0);

            if (couponListResponse?.Result != null && couponListResponse.IsSuccess)
            {
                couponDtos = couponListResponse.Result.ToList();
            }
            else
            {
                TempData["error"] = couponListResponse?.Message;
            }
            return View(couponDtos);
        }

        public IActionResult CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                var createResponse = await couponService.CreateAsync(model);

                if (createResponse?.Result != null && createResponse.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = createResponse?.Message;
                }
            }
            return View(model);
        }


        public async Task<IActionResult> CouponDelete(int couponId)
        {
            var deleteResponse = await couponService.DeleteAsync(couponId);

            if (!deleteResponse.IsSuccess)
            {
                TempData["error"] = deleteResponse?.Message;
            }
            else {
                TempData["success"] = "Coupon deleted successfully";
            }

            return RedirectToAction(nameof(CouponIndex));
        }
    }
}

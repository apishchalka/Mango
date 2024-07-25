using AutoMapper;
using Mango.Web.Extenstions;
using Mango.Web.Models;
using Mango.Web.Models.Cart;
using Mango.Web.Service.IService;
using Mango.Web.Service.IService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService shoppingCartService;
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public CartController(IShoppingCartService shoppingCartService, IOrderService orderService, IMapper mapper)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await GetCart());
        }

        public async Task<IActionResult> Checkout()
        {
            var checkoutCart = mapper.Map<CheckoutCartDto>(await GetCart());
            return View(checkoutCart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var orderValidation = await orderService.ValidateStripeSessionAsync(orderId);

            ConfirmationDto confirmation = new() { OrderId = orderId, IsApproved = false };

            if (orderValidation.IsSuccess)
            {
                confirmation.IsApproved = orderValidation.Result.Status == Enum.OrderStatus.Approved;
            }
            else
            {
                TempData["error"] = orderValidation.Message;
            }

            return View(confirmation);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CheckoutCartDto checkoutCart)
        {
            var cart = await GetCart();

            if (ModelState.IsValid)
            {
                cart.Header.FirstName = checkoutCart.Header.FirstName;
                cart.Header.LastName = checkoutCart.Header.LastName;
                cart.Header.Email = checkoutCart.Header.Email;
                cart.Header.Phone = checkoutCart.Header.Phone;

                var createOrderResponse = await orderService.CreateOrder(cart);

                if (createOrderResponse.IsSuccess)
                {
                    var domain = Request.Scheme + "://" + Request.Host.Value;

                    StripeRequestDto stripeRequest = new()
                    {
                        ApprovedUrl = $"{domain}/cart/confirmation?OrderId={createOrderResponse.Result.OrderId}",
                        CancelUrl = $"{domain}/cart/checkout",
                        OrderHeader = createOrderResponse.Result,
                    };

                    var response = await orderService.CreateStripeSession(stripeRequest);

                    Response.Headers.Append("Location", response.Result.SessionUrl);

                    return new StatusCodeResult(303);
                }
                else
                {

                    TempData["error"] = createOrderResponse.Message;
                }
            }

            return View(checkoutCart);
        }

        public async Task<IActionResult> RemoveDetail(int cartDetailId)
        {
            var cart = await GetCart();
            var detail = cart.Details.Single(x => x.CarDetailId == cartDetailId);

            if (detail != null)
            {
                cart.Details.Remove(detail);
            }

            await shoppingCartService.Upsert(cart);

            TempData["success"] = "Cart updted successfully";

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto shoppingCartDto)
        {
            shoppingCartDto.Details = [];
            var appliedResponse = await shoppingCartService.ApplyCoupon(shoppingCartDto);

            if (appliedResponse.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully.";               
            }
            else
            {
              TempData["error"] = appliedResponse.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost("EmailCart")]
        public async Task<IActionResult> EmailCart(CartDto shoppingCartDto)
        {
            var cart = await GetCart();

            cart.Header.Email = User.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value;

            var emailCartResponse = await shoppingCartService.EmailCart(cart);

            if (emailCartResponse.IsSuccess)
            {
                TempData["success"] = "Cart has been sent!";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {

                TempData["error"] = emailCartResponse.Message;
            }

            return View();
        }

        [HttpPost("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto shoppingCartDto)
        {
            shoppingCartDto.Details = [];
            shoppingCartDto.Header.CouponCode = "";

            var appliedResponse = await shoppingCartService.ApplyCoupon(shoppingCartDto);

            if (appliedResponse.IsSuccess)
            {
                TempData["success"] = "Coupon deleted";
            }
            else
            {

                TempData["error"] = appliedResponse.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        private async Task<CartDto> GetCart()
        {
            var userId = User.GetId();
            var shoppingCartResponse = await shoppingCartService.GetCart(userId);

            if (shoppingCartResponse.IsSuccess && shoppingCartResponse.Result != null)
            {
                return shoppingCartResponse.Result;
            }

            return new CartDto();
        }
    }
}

using AutoMapper;
using Mango.OrderAPI.Dto.Model;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Dto;
using Mango.Services.OrderAPI.Model;
using Mango.Services.OrderAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        private const string currency = "usd";
        private const string stripeMode = "payment";
        private const string _paymentIntentStatusSucceded = "paid";
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public OrderAPIController(AppDbContext dbContext, IMapper mapper, IProductService productService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productService = productService;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDto<OrderHeaderDto>> CreateOrder([FromBody] CartDto cart)
        {
            ResponseDto<OrderHeaderDto> response = new();

            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cart.Header);
                orderHeaderDto.OrderTime = DateTime.UtcNow;
                orderHeaderDto.Status = OrderStatus.Pending;
                orderHeaderDto.Details = _mapper.Map<IEnumerable<OrderDetailDto>>(cart.Details).ToList();

                OrderHeader orderHeader = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbContext.SaveChangesAsync();
                orderHeaderDto.OrderId = orderHeader.OrderId;

                response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto<OrderHeaderDto>> ValidateStripeSession(int orderId)
        {
            ResponseDto<OrderHeaderDto> response = new();
            
            try
            {
                var orderHeader = await _dbContext.OrderHeaders.SingleOrDefaultAsync(x => x.OrderId == orderId);

                if (orderHeader != null)
                {

                    var service = new SessionService();
                    var stripeSession = await service.GetAsync(orderHeader.PaymentSessionId);

                    if (stripeSession.PaymentStatus == _paymentIntentStatusSucceded)
                    {
                        orderHeader.PaymentIntentId = stripeSession.PaymentIntentId;
                        orderHeader.Status = OrderStatus.Approved;
                        await _dbContext.SaveChangesAsync();
                    }

                    response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    response.Message = "Order does not exist";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }


        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto<StripeRequestDto>> CreateStripeSession(StripeRequestDto request)
        {
            ResponseDto<StripeRequestDto> response = new();            
            
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = request.ApprovedUrl,
                    CancelUrl = request.CancelUrl,
                    Discounts = new List<SessionDiscountOptions>() 
                    {
                        new()
                        {
                            Coupon = request.OrderHeader.CouponCode
                        }
                    },
                    LineItems = request.OrderHeader.Details.Select(x =>
                    {
                        return new SessionLineItemOptions()
                        {
                            PriceData = new SessionLineItemPriceDataOptions()
                            {
                                UnitAmount = (long)x.ProductPrice * 100,
                                Currency = currency,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = x.ProductName
                                },
                            },
                            Quantity = x.Count
                        };
                    }).ToList(),


                    Mode = stripeMode,
                };
                
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                request.SessionUrl = session.Url;
                response.Result = request;
                
                var orderHeader = _dbContext.OrderHeaders.First(h => h.OrderId == request.OrderHeader.OrderId);
                orderHeader.PaymentSessionId = session.Id;
                orderHeader.PaymentIntentId = session.PaymentIntentId;

                await _dbContext.SaveChangesAsync();

                return response;

            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
                response.IsSuccess = false;
            }            

            return response;
        }
    }
}

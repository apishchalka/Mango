using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppnigCartAPI.Data;
using Mango.Services.ShoppnigCartAPI.Model;
using Mango.Services.ShoppnigCartAPI.Model.Dto;
using Mango.Services.ShoppnigCartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShoppnigCartAPI.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;

        public ShoppingCartService(AppDbContext dbContext, IConfiguration configuration, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
        }

        public async Task<ResponseDto<bool>> ApplyCoupon(ShoppingCartDto cart)
        {
            var response = new ResponseDto<bool>();

            try
            {

                var dbCart = await _dbContext.CartHeaders.SingleOrDefaultAsync(u => u.UserId == cart.Header.UserId && u.CartHeaderId == cart.Header.CartHeaderId);

                if (dbCart == null)
                {
                    response.Message = "User cart does not exist";
                    response.IsSuccess = false;
                    return response;
                }

                if (!string.IsNullOrEmpty(cart.Header.CouponCode))
                {
                    var coupon = await _couponService.GetCouponAsync(cart.Header.CouponCode!);

                    if (coupon == null)
                    {
                        response.Message = "Coupon not found";
                        response.IsSuccess = false;
                        return response;
                    }
                }

                dbCart.CouponCode = cart.Header.CouponCode;
                _dbContext.CartHeaders.Update(dbCart);
                await _dbContext.SaveChangesAsync();
                response.Result = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto<bool>> EmailCart(ShoppingCartDto cart)
        {
            var response = new ResponseDto<bool>();

            try
            {
                var message = JsonConvert.SerializeObject(cart);
                await _messageBus.SendMessageAsync(message, _configuration.GetValue<string>("ServiceBus:ShoppingCartQueue")!);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto<ShoppingCartDto>> GetCart(string userId)
        {
            var response = new ResponseDto<ShoppingCartDto>();

            try
            {
                ShoppingCartDto shoppingCartDto = new();

                var dbCart = await _dbContext.CartHeaders.SingleOrDefaultAsync(u => u.UserId == userId);

                if (dbCart == null)
                {
                    shoppingCartDto.Header = new()
                    {
                        UserId = userId,
                    };
                    shoppingCartDto.Details = new List<CartDetailsDto>(0);
                }
                else
                {
                    shoppingCartDto.Header = _mapper.Map<CartHeaderDto>(dbCart);
                    var details = await _dbContext.CartDetails.Where(u => u.CartHeaderId == dbCart.CartHeaderId).ToListAsync();
                    shoppingCartDto.Details = _mapper.Map<IEnumerable<CartDetailsDto>>(details);

                    var products = await _productService.GetProducts();

                    foreach (CartDetailsDto detail in shoppingCartDto.Details)
                    {
                        var p = products.SingleOrDefault(x => x.Id == detail.ProductId);
                        detail.Product = p;

                        if (p != null)
                        {
                            shoppingCartDto.Header.CartTotal += (p.Price * detail.Count);
                        }
                    }

                    if (!shoppingCartDto.Header.CouponCode.IsNullOrEmpty())
                    {
                        var coupon = await _couponService.GetCouponAsync(shoppingCartDto.Header.CouponCode);

                        if (coupon != null && shoppingCartDto.Header.CartTotal >= coupon.DiscountAmount)
                        {
                            shoppingCartDto.Header.CartTotal -= coupon.DiscountAmount;
                            shoppingCartDto.Header.Discount = coupon.DiscountAmount;
                        }
                    }
                }

                response.Result = shoppingCartDto;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto<bool>> Upsert(ShoppingCartDto shoppingCartDto)
        {
            var response = new ResponseDto<bool>();

            try
            {
                if (shoppingCartDto.Header.CartHeaderId == 0)
                {
                    CartHeader header = _mapper.Map<CartHeader>(shoppingCartDto.Header);
                    IEnumerable<CartDetails> details = _mapper.Map<IEnumerable<CartDetails>>(shoppingCartDto.Details.ToList());

                    foreach (var detail in details)
                    {
                        detail.Header = header;
                    }

                    _dbContext.CartHeaders.Add(header);
                    _dbContext.CartDetails.AddRange(details);
                }
                else
                {
                    var dbHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == shoppingCartDto.Header.UserId);

                    if (dbHeader == null)
                        throw new ApplicationException("Cart does not exist.");

                    dbHeader.CouponCode = shoppingCartDto.Header.CouponCode;

                    IEnumerable<CartDetails> dtoDetails = _mapper.Map<IEnumerable<CartDetails>>(shoppingCartDto.Details.ToList());
                    var dbDetails = await _dbContext.CartDetails.Where(d => d.CartHeaderId == dbHeader.CartHeaderId).ToListAsync();

                    List<CartDetails> detailsToUpdate = new();
                    List<CartDetails> detailsToDelete = new();
                    List<CartDetails> detailsToAdd = new();

                    foreach (var detail in dtoDetails)
                    {
                        var existed = dbDetails.FirstOrDefault(x => x.CarDetailId == detail.CarDetailId);

                        if (existed != null)
                        {
                            existed.Count = detail.Count;
                            detailsToUpdate.Add(existed);
                        }
                        else
                        {
                            detail.CarDetailId = 0;
                            detail.Header = dbHeader;
                            detailsToAdd.Add(detail);
                        }
                    }

                    detailsToDelete.AddRange(dbDetails.ExceptBy(dtoDetails.Select(d => d.CarDetailId), x => x.CarDetailId));

                    if (detailsToUpdate.Count > 0)
                    {
                        _dbContext.CartDetails.UpdateRange(detailsToUpdate);
                    }

                    if (detailsToAdd.Count > 0)
                    {
                        _dbContext.CartDetails.AddRange(detailsToAdd);
                    }
                    if (detailsToDelete.Count > 0)
                    {
                        _dbContext.CartDetails.RemoveRange(detailsToDelete);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}

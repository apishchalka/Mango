using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Dto;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponApiController : ControllerBase
    {
        AppDbContext _dbContext;
        ResponseDto _responseDto;
        IMapper _mapper;
        public CouponApiController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Models.Coupon> objList = await _dbContext.Coupons.ToListAsync();
                _responseDto.Result = objList;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;

        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var data = await _dbContext.Coupons.FirstOrDefaultAsync(x => x.CouponId == id);

                _responseDto.Result = _mapper.Map<CouponDto>(data);
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ResponseDto> Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Models.Coupon>(couponDto);
                var data = await _dbContext.Coupons.AddAsync(coupon);
                await _dbContext.SaveChangesAsync();

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);

                var options = new CouponCreateOptions
                {
                    AmountOff = (long)couponDto.DiscountAmount * 100,
                    Currency="usd",
                    Id = coupon.CouponCode,
                    Name = coupon.CouponCode
                };
                var service = new CouponService();
                await service.CreateAsync(options);
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var coupon = await _dbContext.Coupons.SingleAsync(x => x.CouponId == id);
                var data = _dbContext.Coupons.Remove(coupon);
                await _dbContext.SaveChangesAsync();

                var service = new CouponService();
                await service.DeleteAsync(coupon.CouponCode);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<ResponseDto> GetByCode(string code)
        {
            try
            {
                var data = await _dbContext.Coupons.FirstOrDefaultAsync(x => x.CouponCode.ToLower() == code);

                _responseDto.Result = _mapper.Map<CouponDto>(data);
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }
    }
}


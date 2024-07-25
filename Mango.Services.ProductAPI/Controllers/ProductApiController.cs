using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        AppDbContext _dbContext;
        ResponseDto _responseDto;
        IMapper _mapper;
        public ProductApiController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Product> objList = await _dbContext.Products.ToListAsync();
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
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var data = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

                _responseDto.Result = _mapper.Map<ProductDto>(data);
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
        public async Task<ResponseDto> Post([FromBody] ProductDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Product>(couponDto);
                var data = await _dbContext.Products.AddAsync(coupon);
                await _dbContext.SaveChangesAsync();

                _responseDto.Result = _mapper.Map<ProductDto>(coupon);
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
                var coupon = await _dbContext.Products.SingleAsync(x => x.Id == id);
                var data = _dbContext.Products.Remove(coupon);
                await _dbContext.SaveChangesAsync();
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

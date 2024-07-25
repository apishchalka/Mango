using Mango.Services.OrderAPI.Dto;
using Mango.Services.OrderAPI.Model;

namespace Mango.OrderAPI.Dto.Model
{
    public class OrderHeaderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string? CouponCode { get; set; }

        public double Discount { get; set; }
        
        public double OrderTotal { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? PaymentSessionId { get; set; }

        public List<OrderDetailDto> Details { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime OrderTime { get; set; }
    }
}

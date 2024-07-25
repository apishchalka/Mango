using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Model
{
    public class OrderHeader
    {
        [Key]
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

        public List<OrderDetail> Details { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime OrderTime { get; set; }
    }
}

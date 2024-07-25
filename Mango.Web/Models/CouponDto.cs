using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }

        [Range(1, 1000)]
        public double DiscountAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int MinAmount { get; set; }
    }
}

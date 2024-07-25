namespace Mango.Web.Models.Cart
{
    public class StripeRequestDto
    {
        public string? SessionId { get; set; }
        public string? SessionUrl { get; set; }
        public string CancelUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}

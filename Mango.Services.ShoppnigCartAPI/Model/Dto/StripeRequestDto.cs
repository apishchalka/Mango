namespace Mango.Web.Models.Cart
{
    public class StripeRequestDto
    {
        public required string SessionId { get; set; }
        public required string SessionUrl { get; set; }
        public required string CancelUrl { get; set; }
        public required string ApprovedUrl { get; set; }
    }
}

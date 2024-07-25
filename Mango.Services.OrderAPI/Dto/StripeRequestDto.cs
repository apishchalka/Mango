using Mango.OrderAPI.Dto.Model;

namespace Mango.Services.OrderAPI.Dto
{
    public class StripeRequestDto
    {
        public required string? SessionId { get; set; }
        public required string? SessionUrl { get; set; }
        public required string CancelUrl { get; set; }
        public required string ApprovedUrl { get; set; }

        public required OrderHeaderDto OrderHeader { get; set; }
    }
}

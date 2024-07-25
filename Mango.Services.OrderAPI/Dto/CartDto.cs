namespace Mango.Services.OrderAPI.Dto
{
    public class CartDto
    {
        public CartHeaderDto Header { get; set; }
        public IList<CartDetailsDto> Details { get; set; }        
    }
}

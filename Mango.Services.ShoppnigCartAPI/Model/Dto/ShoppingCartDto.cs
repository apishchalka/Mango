namespace Mango.Services.ShoppnigCartAPI.Model.Dto
{
    public class ShoppingCartDto
    {
        public CartHeaderDto Header { get; set; }
        public IEnumerable<CartDetailsDto> Details { get; set; }        
    }
}

namespace Mango.Services.EmailAPI.Models.Dtos
{
    public class ShoppingCartDto
    {
        public CartHeaderDto Header { get; set; }
        public IList<CartDetailsDto> Details { get; set; }        
    }
}

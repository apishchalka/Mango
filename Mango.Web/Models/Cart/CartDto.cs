namespace Mango.Web.Service.IService.Dto
{
    public class CartDto
    {
        public CartHeaderDto Header { get; set; }
        public IList<CartDetailsDto> Details { get; set; }        
    }
}

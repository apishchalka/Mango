namespace Mango.Web.Service.IService.Dto
{
    public class CheckoutCartDto
    {
        public CheckoutHeaderDto Header { get; set; }
        public IList<CheckoutDetailDto> Details { get; set; }        
    }
}

using Mango.Web.Models;

namespace Mango.Web.Service.IService.Dto
{
    public class CartDetailsDto
    {
        public int CarDetailId { get; set; }

        public int CartHeaderId { get; set; }

        public CartHeaderDto? Header { get; set; }

        public int ProductId { get; set; }

        public ProductDto? Product { get; set; }

        public int Count { get; set; }
    }
}

namespace Mango.Services.EmailAPI.Models.Dtos
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

using Mango.Services.ShoppnigCartAPI.Model.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppnigCartAPI.Model
{
    public class CartDetails
    {
        [Key]
        public int CarDetailId { get; set; }

        public int CartHeaderId { get; set; }

        [ForeignKey("CartHeaderId")]
        public CartHeader Header { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public ProductDto Product { get; set; }

        public int Count { get; set; }
    }
}

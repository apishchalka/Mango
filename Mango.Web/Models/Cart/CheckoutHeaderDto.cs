using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Service.IService.Dto
{
    public class CheckoutHeaderDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required]
        public string? LastName { get; set; }

        public double CartTotal { get; set; }
        public double Discount { get; set; }
    }
}

namespace Mango.Web.Models
{
    public class ConfirmationDto
    {
        public required int OrderId { get; set; }
        public required bool IsApproved { get; set; }       
    }
}

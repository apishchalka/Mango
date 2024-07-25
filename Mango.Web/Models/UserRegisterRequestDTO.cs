using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class UserRegisterRequestDTO
    {
        public int Id { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Service.IService.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        public double Discount { get; set; }

        public double CartTotal { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required]
        public string? LastName { get; set; }
    }
}

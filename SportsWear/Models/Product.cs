using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace SportsWear.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the category")]
        public int FkCategoryId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the product name")]
        public string ProductName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the product price")]
        public double ProductPrice { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the product quantity")]
        public int ProductQty { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the product size")]
        public string ProductSize { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the gender")]
        public string ProductGender { get; set; }
        public string ProductImage { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the product description")]
        public string ProductDesc { get; set; }
        [NotMapped]
        [DisplayName("Upload Image")]
        public IFormFile ImageFile { set; get; }

        public virtual Category FkCategory { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

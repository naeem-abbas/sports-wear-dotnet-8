using System;
using System.Collections.Generic;

#nullable disable

namespace SportsWear.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkProductId { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }

        public virtual Customer FkCustomer { get; set; }
        public virtual Product FkProduct { get; set; }
    }
}

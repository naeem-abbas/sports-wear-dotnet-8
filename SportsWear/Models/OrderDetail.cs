using System;
using System.Collections.Generic;

#nullable disable

namespace SportsWear.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int FkOrderId { get; set; }
        public int FkProductId { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }

        public virtual Order FkOrder { get; set; }
        public virtual Product FkProduct { get; set; }
    }
}

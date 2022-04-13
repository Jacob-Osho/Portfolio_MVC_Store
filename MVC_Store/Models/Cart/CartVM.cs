using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Cart
{
    public class CartVM
    {
        public int PorductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public string Image { get; set; }
    }
}
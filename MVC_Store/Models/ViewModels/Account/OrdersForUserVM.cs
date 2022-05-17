using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.ViewModels.Account
{
    public class OrdersForUserVM
    {

        [Display(Name = "Order number :")]
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }
        [Display(Name = " Order details :")]
        public Dictionary<string, int> ProductsAndQty { get; set; }
        [Display(Name = " Date of creation :")]
        public DateTime CreatedAt { get; set; }
    }
}
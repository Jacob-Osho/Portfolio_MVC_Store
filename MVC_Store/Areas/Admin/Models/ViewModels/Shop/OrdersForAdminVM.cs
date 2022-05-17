using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_Store.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        [Display(Name = "Order number :")]
        public int OrderNumber { get; set; }
        [Display(Name = " Username :")]
        public string UserName { get; set; }
        public decimal Total { get; set; }
        [Display(Name = " Order details :")]
        public Dictionary<string,int> ProductsAndQty { get; set; }
        [Display(Name = " Date of creation :")]
        public DateTime CreatedAt { get; set; }
    }
}
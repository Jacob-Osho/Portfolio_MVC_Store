using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//6
namespace MVC_Store.Models.ViewModels.Pages
{
    public class SideBarVM
    {
        public SideBarVM()
        {

        }
        public SideBarVM(SideBarDTO row)
        {
            Id = row.Id;
            Body = row.Body; 
        }
        public int Id { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}
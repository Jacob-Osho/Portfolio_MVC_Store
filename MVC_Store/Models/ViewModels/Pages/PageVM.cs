﻿using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Models.ViewModels.Pages
{
    public class PageVM
    {
        public PageVM()
        {

        }
        public PageVM(PagesDTO pages)
        {
            Id = pages.Id;
            Title = pages.Title;
            Body = pages.Body;
            Slug = pages.Slug;
            Sorting = pages.Sorting;
            HasSidebar = pages.HasSidebar;

        }
        public int Id { get; set; }
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string Title { get; set; }
        [Required]
        [AllowHtml]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
        [Display(Name ="SideBar")]
        public bool HasSidebar { get; set; }
    }
}
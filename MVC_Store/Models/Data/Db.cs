using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Data
{
    public class Db :DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }
        //6
        public DbSet<SideBarDTO> Sidebras { get; set; }
        //8
        public DbSet<CategoryDTO> Categories { get; set; }
        //11
        public DbSet<ProductDTO> Products { get; set; }
    }
}
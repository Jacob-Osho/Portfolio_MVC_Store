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
        public DbSet<EventDTO> Events { get; set; }
        //22
        public DbSet<UserDTO> Users { get; set; }

        public DbSet<RoleDTO> Roles { get; set; }

        //23
        public DbSet<UserRoleDTO> UserRoles { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
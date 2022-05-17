﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Data
{
    [Table("tblRoles")]
    public class RoleDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
using MVC_Store.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Data
{
    [Table("tblUsers")]
    public class UserDTO
    {
        public UserDTO()
        {

        }
        public UserDTO(UserVM row)
        {
            Id = row.Id;
            UserName = row.UserName;
            LastName = row.LastName;
            EmailAdress = row.EmailAdress;
            UserName = row.UserName;
            Password = row.Password;
            
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

    }
}
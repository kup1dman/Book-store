using BookStore.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models.ViewModels.Account
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel() { }

        public UserProfileViewModel(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            Username = row.Username;
            Password = row.Password;
        }

        public int Id { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayName("Логин")]
        public string Username { get; set; }

        [DisplayName("Пароль")]
        public string Password { get; set; }

        [DisplayName("Подтвердите пароль")]
        public string ConfirmPassword { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
        public long CreateTime { get; set; }
        public bool Enable { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
    }
    public class UserLoginModel
    {
        public string Account { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string FireBaseToken { get; set; }
    }
    public class UserListViewModel
    {
        public List<User> ListUser { get; set; }
        public int TotalPage { get; set; }
    }
    public class UserRequest
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
    public class UserLoginPost
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
    public class UserUpdateModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string SearchName { get; set; }
        public string Avatar { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
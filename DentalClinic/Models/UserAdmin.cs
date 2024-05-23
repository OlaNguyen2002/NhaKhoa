using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class UserAdmin
    {
        public string UserAdminId { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public long CreateTime { get; set; }
        public class EnumRole
        {
            public const string ADMIN = "ADMIN";
            public const string STAFF = "STAFF";
        }

    }
    //public class UserLoginPost
    //{
    //    public string Account { get; set; }
    //    public string Password { get; set; }
    //}
    public class UserAdminChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
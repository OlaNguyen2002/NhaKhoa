using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DentalClinic.Providers
{
    public class UserProvider
    {
        public static User GetUserFromRequestHeader(HttpRequestMessage request, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            if (request == null) return null;
            string token = request.Headers.GetValues("Authorization").FirstOrDefault();
            if (string.IsNullOrEmpty(token)) return null;

            UserService userService = new UserService(connect);
            User user = userService.GetUserByToken(token, transaction);

            return user;
        }

        public static UserAdmin GetUserAdminFromRequestHeader(HttpRequestMessage request, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            if (request == null) return null;
            string token = request.Headers.GetValues("Authorization").FirstOrDefault();
            if (string.IsNullOrEmpty(token)) return null;

            UserAdminService userAdminService = new UserAdminService(connect);
            UserAdmin userAdmin = userAdminService.GetUserAdminByToken(token, transaction);

            return userAdmin;
        }
        public static bool CheckTimeOrder(int minTime, int maxTime, List<TimeDoctor> lstimeDoctors, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            bool check = true;
            for (int i = 0; i < lstimeDoctors.Count; i++)
            {
                if ((minTime >= lstimeDoctors[i].MinTime && minTime < lstimeDoctors[i].MaxTime) || (maxTime > lstimeDoctors[i].MinTime && maxTime <= lstimeDoctors[i].MaxTime))
                {
                    check = false;
                }
            }
            return check;
        }
    }
}
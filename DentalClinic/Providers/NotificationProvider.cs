using DentalClinic.Models;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Providers
{
    public class NotificationProvider
    {
        public static bool CreateNotification(Notification model, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            try
            {
                NotificationService notificationService = new NotificationService(connect);
                if (!notificationService.InsertNotification(model, transaction)) throw new Exception();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
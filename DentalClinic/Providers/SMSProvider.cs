using DentalClinic.Models;
using DentalClinic.Services;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DentalClinic.Providers
{
    public class SMSProvider
    {
        public static bool SendOTPViaEmail(string email, string code, string title, string message)
        {
            try
            {
                System.Net.Mail.SmtpClient clientDetails = new System.Net.Mail.SmtpClient();
                clientDetails.Port = 587;
                clientDetails.Host = "smtp.gmail.com";
                clientDetails.EnableSsl = true;
                clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
                clientDetails.UseDefaultCredentials = false;
                clientDetails.Credentials = new NetworkCredential("thaovu13092001@gmail.com", "sehhnofdtuxvidol");

                //Message Details
                MailMessage mailDetails = new MailMessage();
                mailDetails.From = new MailAddress("thaovu13092001@gmail.com");
                mailDetails.To.Add(email);
                mailDetails.Subject = "[Nha khoa Hữu Tuấn] " + title;
                mailDetails.IsBodyHtml = true;
                string path = HttpContext.Current.Server.MapPath("~/Content/files/emailtemplates/Confirm.txt");
                string body = System.IO.File.ReadAllText(path);
                mailDetails.Body = body.Replace("{{Name}}", email);
                //mailDetails.Body = body;
                clientDetails.Send(mailDetails);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public class SMSResponse
        {
            public string CodeResult { get; set; }
            public string CountRegenerate { get; set; }
            public string SMSID { get; set; }
        }
    }
}
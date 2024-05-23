using Dapper;
using DentalClinic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DentalClinic.Services
{
    public class AppointmentStatusService : BaseService
    {
        public AppointmentStatusService() : base() { }
        public AppointmentStatusService(IDbConnection db) : base(db) { }

        public bool CreateAppointmentStatus(AppointmentStatus model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[appointment_status]([AppointmentStatusId],[UserAppointmentId],[Status],[CreateTime]) " +
                "VALUES (@AppointmentStatusId,@UserAppointmentId,@Status,@CreateTime)";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
    }
}
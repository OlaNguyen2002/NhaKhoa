using DentalClinic.Models;
using DentalClinic.Providers;
using DentalClinic.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;

namespace DentalClinic.Areas.Admin.ApiControllers
{
    public class SendMailScheduleWorker 
    {
        //public async Task Execute()
        //{
        //    Console.WriteLine("Doing something.");
        //    HostingEnvironment.QueueBackgroundWorkItem(ct => FireAndForgetMethodAsync());
        //    //HostingEnvironment.QueueBackgroundWorkItem(ct => FireAndForgetMethod());
        //    //HostingEnvironment.QueueBackgroundWorkItem(ct => FaultyFireAndForgetMethod());
        //}

        //private async Task FireAndForgetMethodAsync()
        //{
        //    Debug.WriteLine($"Started running an asynchronous background work item... {DateTime.UtcNow}");

        //    await Task.Delay(1000); // Pretend we are doing something for 5s
        //}

        //private void FireAndForgetMethod()
        //{
        //    Debug.WriteLine("Started running a background work item...");

        //    Thread.Sleep(5000); // Pretend we are doing something for 5s

        //    Debug.WriteLine("Finished running that.");
        //}

        //private void FaultyFireAndForgetMethod()
        //{
        //    throw new Exception("I'm failing just to make a point.");
        //}
    }
}

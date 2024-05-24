using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Web;
using System.Net.Http;
using System.Net.WebSockets;
using System.Web.Mvc;
using Microsoft.Web.WebSockets;
using System.Web.Script.Serialization;
using Microsoft.ServiceModel.WebSockets;
using System.Runtime.InteropServices;

namespace DentalClinic.ApiControllers
{
    public class ChatController:ApiBaseController
    {
      //private static WebSocketCollection connections = new WebSocketCollection();
        protected static Dictionary<string, WebSocketHandler> listWebSocketUser = new Dictionary<string, WebSocketHandler>();
        protected static Dictionary<string, WebSocketHandler> listWebSocketAdmin = new Dictionary<string, WebSocketHandler>();
        [HttpGet]
        [AllowAnonymous]


        public async Task<HttpResponseMessage> Get(string account, string type)
        {
            //User user = SystemProvider.GetUserFromRequestHeader(Request.Headers);


            if (HttpContext.Current.IsWebSocketRequest)
            {
                var noteHandler = new ChatSocketHandler(account, type);
                HttpContext.Current.AcceptWebSocketRequest(noteHandler);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }


        class ChatSocketHandler : WebSocketHandler {
            public string account;
            public string type;
            public string typeAdmin = "ADMIN";
            public string typeUser = "USER";
            public ChatSocketHandler(string _account, string _type) { this.account = _account; this.type = _type; }
            public override void OnOpen()
            {


                if (type == typeUser)
                {
                    if (!listWebSocketUser.ContainsKey(account))
                    {
                        listWebSocketUser.Add(account, this);
                    }
                    else
                    {
                        listWebSocketUser[account] = this;
                    }
                }
                else if (type == typeAdmin)
                {
                    if (!listWebSocketAdmin.ContainsKey(account))
                    {
                        listWebSocketAdmin.Add(account, this);
                    }
                    else
                    {
                        listWebSocketAdmin[account] = this;
                    }
                }
            }
            public override void OnMessage(string message)
            {
                MessageModel socketAction = new JavaScriptSerializer().Deserialize<MessageModel>(message);


                // string retunrAction = new JavaScriptSerializer().Serialize(socketAction);

                //  listWebSocketUser[socketAction.receiveAccount].Send(message);

                if (type == typeUser)
                {
                    foreach (var connection in listWebSocketAdmin.Values)
                    {
                        connection.Send(message);
                    }
                    listWebSocketUser[account].Send(message);
                }
                else if (type == typeAdmin)
                {
                    if (listWebSocketUser.ContainsKey(socketAction.receiveAccount))
                    {
                        listWebSocketUser[socketAction.receiveAccount].Send(message);
                    }
                    foreach (var connection in listWebSocketAdmin.Values)
                    {
                        connection.Send(message);
                    }
                }



            }
        }



    }
    public class MessageModel
    {
        public string account { get; set; }
        public string message { get; set; }
        public string receiveAccount { get; set; }
        public string senderType { get; set; }
    }
}

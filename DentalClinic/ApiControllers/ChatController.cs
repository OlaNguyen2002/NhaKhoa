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

namespace DentalClinic.ApiControllers
{
    public class ChatController:ApiBaseController
    {
      //private static WebSocketCollection connections = new WebSocketCollection();
        protected static Dictionary<string, WebSocketHandler> listWebSocketUser = new Dictionary<string, WebSocketHandler>();

        [HttpGet]
        [AllowAnonymous]


        public async Task<HttpResponseMessage> Get(string account)
        {
            //User user = SystemProvider.GetUserFromRequestHeader(Request.Headers);


            if (HttpContext.Current.IsWebSocketRequest)
            {
                var noteHandler = new ChatSocketHandler(account);
                HttpContext.Current.AcceptWebSocketRequest(noteHandler);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }


        class ChatSocketHandler : WebSocketHandler {
            public string account;
            public ChatSocketHandler(string _account) { this.account = _account; }
            public override void OnOpen()
            {
                listWebSocketUser.Add(account, this);
            }
            public override void OnMessage(string message)
            {
                MessageModel socketAction = new JavaScriptSerializer().Deserialize<MessageModel>(message);
           

                string retunrAction = new JavaScriptSerializer().Serialize(socketAction);

                listWebSocketUser[socketAction.receiveAccount].Send(message);
                
               /* foreach (var connection in listWebSocketUser)
                {
                
                    connection.Value.Send(message);
                }
                */
            }
        }



    }
    public class MessageModel
    {
        public string account { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public string receiveAccount { get; set; }
    }
}

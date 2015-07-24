using System;
using System.Collections;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

using SingleKinect;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public void UpdateModel(ShapeModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            Clients.AllExcept(clientModel.LastUpdatedBy).updateShape(clientModel);
        }
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }

    public class ShapeModel
    {
        [JsonProperty("left")]
        public double Left { get; set; }
        [JsonProperty("top")]
        public double Top { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }
}
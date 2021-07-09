using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Hubs
{
    public class StronglyTypeHub : Hub<IStronglyTypeHub>
    {
        public async Task Send(string message)
        {
            await Clients.All.NewMessage(message);
        }

        public async Task Send1(string connectionId, string message)
        {
            await Clients.Client(connectionId).NewMessage(message);
        }
    }
}

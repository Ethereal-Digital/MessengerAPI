using ChatroomAPI.Database;
using ChatroomAPI.Services;
using ChatroomAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Hubs
{
    //[EnableCors("MyPolicy")]
    public class ChatHub : Hub
    {
        private IHttpContextAccessor _hcontext;
        private ChatContext _chatContext { get; set; }

        public ChatHub(IHttpContextAccessor haccess, ChatContext chatContext)
        {
            _hcontext = haccess;
            _chatContext = chatContext;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            var test = Context.User;
            var test1 = Context.User.Claims;
            var test2 = Context.UserIdentifier;
            var test3 = _hcontext;
            var test4 = _hcontext.HttpContext.User.Claims;
            var test5 = _hcontext.HttpContext.Request;

            var test9 = _chatContext.users;
            var test6 = _chatContext.messages;
            var test7 = _chatContext.users.Where(x => x.Id == 1).FirstOrDefault();
            var test10 = _chatContext.messageTypes;
            var test11 = _chatContext.participants;
            var test12 = _chatContext.rooms;


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception e)
        {
            var check = Context.ConnectionId;
            return base.OnDisconnectedAsync(e);
        }

        //public override Task OnReconnected()
        //{
        //    return base.OnReconnected();
        //}
    }
}

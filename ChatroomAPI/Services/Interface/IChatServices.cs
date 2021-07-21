using ChatroomAPI.Middleware;
using ChatroomAPI.Model;
using ChatroomAPI.Model.Dto;
using ChatroomAPI.Model.Frontend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Services.Interface
{
    public interface IChatServices
    {
        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo);

        public Task<List<RoomDto>> GetRoomList();
        public Task<List<MessageDto>> GetMessageHistory(UserMessageHistory userMessageHistory);
      
        public Task SendMessage(Message message);
        public Task SendFileMessage(IFormFile file, Message message);

        public Task RejoinRoom(UserConnectionInfo userConnectionInfo);
        public Task RoomAction(UserRoomInfo userRoomInfo);

    }
}

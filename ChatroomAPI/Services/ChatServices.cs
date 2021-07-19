using ChatroomAPI.Controllers;
using ChatroomAPI.Database;
using ChatroomAPI.Middleware;
using ChatroomAPI.Model;
using ChatroomAPI.Model.Dto;
using ChatroomAPI.Model.Enum;
using ChatroomAPI.Model.Frontend;
using ChatroomAPI.Model.Hubs;
using ChatroomAPI.Repositories.Interface;
using ChatroomAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Services
{
    public class ChatServices : IChatServices
    {
        private ChatMiddleware _chatManager { get; set; } = new ChatMiddleware();
        private readonly IHubContext<ChatHub> _hubContext;
        private IChatRepository _chatRepository { get; set; }
        private IFileServices _fileService { get; set; }

        public ChatServices([NotNull] IHubContext<ChatHub> chatHub, IFileServices fileServices, IChatRepository chatRepository)
        {
            _hubContext = chatHub;
            _fileService = fileServices;
            _chatRepository = chatRepository;
        }

        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo)
        {
            _chatManager.UpdateUserHubConnection(newUserInfo);
        }

        public async Task<List<RoomDto>> GetRoomList()
        {
            List<RoomDto> Rooms = await _chatRepository.GetRoomList();
            return Rooms;
        }
        public async Task<List<MessageDto>> GetMessageHistory(UserMessageHistory userMessageHistory)
        {
            return  await _chatRepository.GetMessageHistory(userMessageHistory);
        }
        public async Task<List<MessageDto>> GetGroupMessageHistory(UserMessageHistory userMessageHistory)
        {
            return await _chatRepository.GetGroupMessageHistory(userMessageHistory);
        }
       
        #region Send Message / File
        public async Task SendMessage(Message message)
        {
            var connectionId = _chatManager.GetUserHubConnectionId(message.ReceiverUID);
            var selfConnectionId = _chatManager.GetUserHubConnectionId(message.SenderUID);

            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("PrivateMessage", message);
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("PrivateMessage", message);
                await UpdateMessageHistory(message);
            }
            else
            {
                message.ReceiverUID = null;
                message.MessageBody = $"NOTE: User is not online";
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("PrivateMessage", message);
            }
        }
        public async Task SendFileMessage(IFormFile file, Message message)
        {
            await _fileService.SaveFile(file, message);

            //var receiverName = _chatManager.GetUserInformation(message.ReceiverUID);
            var connectionId = _chatManager.GetUserHubConnectionId(message.ReceiverUID);
            var selfConnectionId = _chatManager.GetUserHubConnectionId(message.SenderUID);

            if (connectionId != null)
            {
                //message.Receiver = receiverName.UserName;
                await _hubContext.Clients.Client(connectionId).SendAsync("PrivateMessage", message);
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("PrivateMessage", message);
                await UpdateMessageHistory(message);
            }
            else
            {
                message.ReceiverUID = null;
                message.MessageBody = $"NOTE: User is not online";
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("PrivateMessage", message);
            }
        }
        public async Task SendMessageToAll(Message message)
        {
            await _hubContext.Clients.All.SendAsync("PublicMessage", message);
            await UpdateMessageHistory(message);
        }
        public async Task SendFileMessageToAll(IFormFile file, Message message)
        {
            await _fileService.SaveFile(file, message);
            await _hubContext.Clients.All.SendAsync("PublicMessage", message);
            await UpdateMessageHistory(message);
        }
        public async Task SendMessageToRoom(Message RoomMessage)
        {
            if (_chatRepository.IsUserInRoom(RoomMessage.SenderUID, RoomMessage.RoomName) == false)
                return;

            await _hubContext.Clients.Group(RoomMessage.RoomName).SendAsync("RoomMessage", RoomMessage);
            await UpdateMessageHistory(RoomMessage);
        }
        public async Task SendFileMessageToRoom(IFormFile file, Message message)
        {
            if (_chatRepository.IsUserInRoom(message.SenderUID, message.RoomName) == false)
                return;

            await _fileService.SaveFile(file, message);
            await _hubContext.Clients.Group(message.RoomName).SendAsync("RoomMessage", message);
            await UpdateMessageHistory(message);
        }
        #endregion

        #region Room function
        public async Task RejoinRoom(UserConnectionInfo userConnectionInfo)
        {
            var rooms = _chatRepository.GetRoom(userConnectionInfo.UserUID);

            if(rooms != null)
            {
                foreach(var room in rooms)
                {
                    await _hubContext.Groups.AddToGroupAsync(userConnectionInfo.ConnectionId, room.Name);
                }
            } 
        }
        public async Task JoinRoom(UserRoomInfo userRoomInfo)
        {
            var connectionId = _chatManager.GetUserHubConnectionId(userRoomInfo.UserUID);
            await _hubContext.Groups.AddToGroupAsync(connectionId, userRoomInfo.RoomName);
            await UpdateUserRoom(userRoomInfo, RoomAction.Join);
        }
        public async Task ExitRoom(UserRoomInfo userRoomInfo)
        {
            var connectionId = _chatManager.GetUserHubConnectionId(userRoomInfo.UserUID);
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, userRoomInfo.RoomName);
            await UpdateUserRoom(userRoomInfo, RoomAction.Exit);
        }
        private async Task UpdateUserRoom(UserRoomInfo userRoomInfo, RoomAction mode)
        {
            ParticipantDto participantDto = new ParticipantDto();

            switch (mode)
            {
                case RoomAction.Join:
                    participantDto.UserUID = userRoomInfo.UserUID;
                    participantDto.RoomId = _chatRepository.GetRoomId(userRoomInfo.RoomName);
                    await _chatRepository.JoinRoom(participantDto);
                    break;

                case RoomAction.Exit:
                    participantDto.UserUID = userRoomInfo.UserUID;
                    participantDto.RoomId = _chatRepository.GetRoomId(userRoomInfo.RoomName);
                    await _chatRepository.ExitRoom(participantDto);
                    break;

                default: break;
            }
        }
        #endregion

        #region Transform Model to Dto
        private async Task UpdateMessageHistory(Message newMessage)
        {
            MessageDto messageDto = new MessageDto();
            messageDto.UID = newMessage.UID;
            messageDto.SenderUID = newMessage.SenderUID;
            messageDto.ReceiverUID = newMessage.ReceiverUID == null ? null : newMessage.ReceiverUID;
            messageDto.RoomId = newMessage.RoomName == null ? null :_chatRepository.GetRoomId(newMessage.RoomName);
            messageDto.MessageTypeId = newMessage.MessageTypeId;
            messageDto.MessageBody = newMessage.MessageBody;
            messageDto.CreatedDate = newMessage.CreatedDate;

            await _chatRepository.UpdateMessageHistory(messageDto);
        }
        #endregion

    }
}

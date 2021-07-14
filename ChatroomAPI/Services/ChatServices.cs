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
        private IChatRepository _chatRepository { get; set; }
        private readonly IHubContext<ChatHub> _hubContext;
        private FileServices _fileService { get; set; }

        public ChatServices([NotNull] IHubContext<ChatHub> chatHub, FileServices fileServices, IChatRepository chatRepository)
        {
            _hubContext = chatHub;
            _fileService = fileServices;
            _chatRepository = chatRepository;
       
        }

        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo)
        {
            _chatManager.UpdateUserHubConnection(newUserInfo);
        }

        public List<RoomDto> GetRoomList()
        {
            List<RoomDto> Rooms = _chatRepository.GetRoomList();
            return Rooms;
        }

        public async Task<List<MessageDto>> GetMessageHistory(UserMessageHistory userMessageHistory)
        {
            return await _chatRepository.GetMessageHistory(userMessageHistory.SenderUID, userMessageHistory.ReceiverUID);
        }

        public async Task <List<MessageDto>> GetGroupMessageHistory(UserGroupMessageHistory userMessageHistory)
        {
            return await _chatRepository.GetGroupMessageHistory(userMessageHistory.SenderUID, userMessageHistory.RoomName);
        }

        public async Task SendFileMessageToAll(IFormFile file, Message message)
        {
            await _fileService.SaveFile(file);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            //await UpdateMessageHistory(message);
        }

        public async Task SendFileMessage(IFormFile file, Message message)
        {
            await _fileService.SaveFile(file);

            var receiverName = _chatManager.GetUserInformation(message.ReceiverUID);
            var connectionId = _chatManager.GetUserHubConnectionId(message.ReceiverUID);
            var selfConnectionId = _chatManager.GetUserHubConnectionId(message.SenderUID);

            if (receiverName != null)
            {
                message.Receiver = receiverName.UserName;

                await _hubContext.Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", message);
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("ReceivePrivateMessage", message);
                await UpdateMessageHistory(message);
            }
            else
            {
                message.MessageBody = $"NOTE: User is not online";
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("ReceivePrivateMessage", message);
            }

        }

        public async Task SendMessage(Message message)
        {
            var receiverName = _chatManager.GetUserInformation(message.ReceiverUID);
            var connectionId = _chatManager.GetUserHubConnectionId(message.ReceiverUID);
            var selfConnectionId = _chatManager.GetUserHubConnectionId(message.SenderUID);

            if (receiverName != null)
            {
                message.Receiver = receiverName.UserName;

                await _hubContext.Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", message);
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("ReceivePrivateMessage", message);
                await UpdateMessageHistory(message);
            }
            else
            {
                message.MessageBody = $"NOTE: User is not online";
                await _hubContext.Clients.Client(selfConnectionId).SendAsync("ReceivePrivateMessage", message);
            }
            
        }

        public async Task SendMessageToRoom(MessageToRoom RoomMessage)
        {
            await _hubContext.Clients.Group(RoomMessage.RoomName).SendAsync("GroupReceiveMessage", RoomMessage);
            await UpdateMessageHistory(RoomMessage);
        }

        public async Task SendMessageToAll(Message message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            await UpdateMessageHistory(message);
        }

        public async Task RejoinRoom(UserConnectionInfo userConnectionInfo)
        {
            var room = _chatRepository.GetRoom(userConnectionInfo.UserUID);

            if(room != null)
                await _hubContext.Groups.AddToGroupAsync(userConnectionInfo.ConnectionId, room.Name);
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

        #region Transform Model to Dto

        private async Task UpdateMessageHistory(Message newMessage)
        {
            MessageDto messageDto = new MessageDto();
            messageDto.UID = Guid.NewGuid().ToString();
            messageDto.SenderUID = newMessage.SenderUID;
            messageDto.ReceiverUID = newMessage.ReceiverUID;
            messageDto.MessageTypeId = newMessage.MessageTypeId;
            messageDto.MessageBody = newMessage.MessageBody;
            messageDto.CreatedDate = newMessage.CreatedDate;

            await _chatRepository.UpdateMessageHistory(messageDto);
        }

        private async Task UpdateMessageHistory(MessageToRoom newMessage)
        {
            MessageDto messageDto = new MessageDto();
            messageDto.UID = Guid.NewGuid().ToString();
            messageDto.SenderUID = newMessage.SenderUID;
            messageDto.RoomId = _chatRepository.GetRoomId(newMessage.RoomName);
            messageDto.MessageTypeId = newMessage.MessageTypeId;
            messageDto.MessageBody = newMessage.MessageBody;
            messageDto.CreatedDate = newMessage.CreatedDate;

            await _chatRepository.UpdateMessageHistory(messageDto);
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

    }
}

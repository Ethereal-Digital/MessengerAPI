using ChatroomAPI.Database;
using ChatroomAPI.Model.Dto;
using ChatroomAPI.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatroomAPI.Model.Frontend;

namespace ChatroomAPI.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private ChatContext _chatContext { get; set; }

        public ChatRepository(ChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        public async Task UpdateMessageHistory(MessageDto newMessage)
        {
            await _chatContext.messages.AddAsync(newMessage);
            await _chatContext.SaveChangesAsync();
        }

        public async Task<MessageDto> GetMessage(string messageUID)
        {
            return await _chatContext.messages.Where(x => x.UID == messageUID).FirstOrDefaultAsync();
        }

        public async Task<List<MessageDto>> GetMessageHistory(UserMessageHistory messageHistory)
        {
            List<MessageDto> messages = new List<MessageDto>();
            if (messageHistory.ReceiverUID != "All")
            {
                messages = await _chatContext.messages.Where(x => x.SenderUID == messageHistory.SenderUID && x.ReceiverUID == messageHistory.ReceiverUID
                                                                || x.SenderUID == messageHistory.ReceiverUID && x.ReceiverUID == messageHistory.SenderUID)
                                                             .OrderBy(x => x.CreatedDate)
                                                             .Skip(messageHistory.ItemSize * messageHistory.Counter)
                                                             .Take(messageHistory.ItemSize)
                                                             .AsNoTracking()
                                                             .ToListAsync();
            }
            else
            {
                messages =  await _chatContext.messages.Where(x => x.ReceiverUID == messageHistory.ReceiverUID).OrderBy(x => x.CreatedDate).AsNoTracking().ToListAsync();
            }
            return messages;
        }

        public async Task<List<MessageDto>> GetGroupMessageHistory(UserMessageHistory userMessageHistory)
        {
            List<MessageDto> messages = new List<MessageDto>();
            messages = await _chatContext.messages.Where(x => x.RoomId == GetRoomId(userMessageHistory.RoomName))
                                                        .OrderBy(x => x.CreatedDate)
                                                        .Skip(userMessageHistory.ItemSize * userMessageHistory.Counter)
                                                        .Take(userMessageHistory.ItemSize)
                                                        .AsNoTracking()
                                                        .ToListAsync();
            return messages;
        }

        public List<RoomDto> GetRoom(string participantUID)
        {
            var room = (from rooms in _chatContext.rooms
                        join participant in _chatContext.participants on rooms.Id equals participant.RoomId
                        where participant.UserUID == participantUID
                        select rooms)
                        .AsNoTracking()
                        .ToList();
            return room;
        }

        public int GetRoomId(string roomName)
        {
            return _chatContext.rooms.AsNoTracking().Where(x => x.Name == roomName).Select(x => x.Id).FirstOrDefault();
        }

        public string GetRoomName(int roomId)
        {
            return _chatContext.rooms.AsNoTracking().Where(x => x.Id == roomId).Select(x => x.Name).FirstOrDefault();
        }

        public bool IsUserInRoom(string participantUID, string roomName)
        {
            return _chatContext.participants.Where(x => x.UserUID == participantUID && x.RoomId == GetRoomId(roomName)).FirstOrDefault() == null; 
        }

        public bool IsUserInRoom(string participantUID, int roomId)
        {
            return _chatContext.participants.Where(x => x.UserUID == participantUID && x.RoomId == roomId).FirstOrDefault() == null;
        }

        public async Task<List<RoomDto>> GetRoomList()
        {
            List<RoomDto> Rooms = await _chatContext.rooms.AsNoTracking().ToListAsync();
            return Rooms;
        }

        public async Task JoinRoom(ParticipantDto participantDto)
        {
            if (IsUserInRoom(participantDto.UserUID, participantDto.RoomId) == true) return;

            await _chatContext.participants.AddAsync(participantDto);
            await _chatContext.SaveChangesAsync();
        }

        public async Task ExitRoom(ParticipantDto participantDto)
        {
            if (IsUserInRoom(participantDto.UserUID, participantDto.RoomId) == false) return;

            var user = _chatContext.participants.Where(x => x.RoomId == participantDto.RoomId
                                                        && x.UserUID == participantDto.UserUID).FirstOrDefault();
            _chatContext.participants.Remove(user);
            await _chatContext.SaveChangesAsync();
        }
    }
}

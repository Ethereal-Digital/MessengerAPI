using ChatroomAPI.Database;
using ChatroomAPI.Model.Dto;
using ChatroomAPI.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<MessageDto>> GetMessageHistory(string senderUID, string receiverUID)
        {
            List<MessageDto> messages = new List<MessageDto>();
            if (receiverUID != "All")
            {
                messages = await _chatContext.messages.AsNoTracking().Where(x => x.SenderUID == senderUID && x.ReceiverUID == receiverUID
                                                                || x.SenderUID == receiverUID && x.ReceiverUID == senderUID)
                                                             .OrderBy(x => x.CreatedDate).ToListAsync();
            }
            else
            {
                messages = await _chatContext.messages.AsNoTracking().Where(x => x.ReceiverUID == receiverUID).OrderBy(x => x.CreatedDate).ToListAsync();
            }
            return messages;
        }

        public async Task<List<MessageDto>> GetGroupMessageHistory(string senderUID, string roomName)
        {
            List<MessageDto> messages = new List<MessageDto>();
            messages = await _chatContext.messages.AsNoTracking().Where(x => x.RoomId == GetRoomId(roomName)).ToListAsync();
            return messages;
        }

        public async Task JoinRoom(ParticipantDto participantDto)
        {
            await _chatContext.participants.AddAsync(participantDto);
            await _chatContext.SaveChangesAsync();
        }

        public async Task ExitRoom(ParticipantDto participantDto)
        {
            var user = _chatContext.participants.Where(x => x.RoomId == participantDto.RoomId
                                                        && x.UserUID == participantDto.UserUID).FirstOrDefault();
            _chatContext.participants.Remove(user);
            await _chatContext.SaveChangesAsync();

        }

        public RoomDto GetRoom(string ParticipantUID)
        {
            var room = (from rooms in _chatContext.rooms
                         join participant in _chatContext.participants on rooms.Id equals participant.RoomId
                         where participant.UserUID == ParticipantUID
                         select rooms)
                      .AsNoTracking()
                      .FirstOrDefault();
            return room;
        }

        public int GetRoomId(string roomName)
        {
            int index = _chatContext.rooms.AsNoTracking().Where(x => x.Name == roomName).Select(x => x.Id).FirstOrDefault();
            return index;
        }

        public string GetRoomName(int RoomId)
        {
            string name = _chatContext.rooms.AsNoTracking().Where(x => x.Id == RoomId).Select(x => x.Description).FirstOrDefault();
            return name;
        }

        public List<RoomDto> GetRoomList()
        {
            List<RoomDto> Rooms = _chatContext.rooms.AsNoTracking().ToList();
            return Rooms;
        }
    }
}

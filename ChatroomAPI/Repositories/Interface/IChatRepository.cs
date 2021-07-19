using ChatroomAPI.Model.Dto;
using ChatroomAPI.Model.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Repositories.Interface
{
    public interface IChatRepository
    {
        public int GetRoomId(string roomName);
        public string GetRoomName(int RoomId);

        public List<RoomDto> GetRoom(string participantUID);
        public Task<MessageDto> GetMessage(string messageUID);
        public Task<List<RoomDto>> GetRoomList();
        public Task<List<MessageDto>> GetMessageHistory(UserMessageHistory messageHistory);
        public Task<List<MessageDto>> GetGroupMessageHistory(UserMessageHistory userMessageHistory);

        public Task UpdateMessageHistory(MessageDto newMessage);
        public Task JoinRoom(ParticipantDto participantDto);
        public Task ExitRoom(ParticipantDto participantDto);
        public bool IsUserInRoom(string participantUID, string roomName);
        public bool IsUserInRoom(string participantUID, int roomId);
    }
}

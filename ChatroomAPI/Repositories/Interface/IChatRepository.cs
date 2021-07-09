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
        public RoomDto GetRoom(string ParticipantUID);

        public List<RoomDto> GetRoomList();
        public Task<List<MessageDto>> GetMessageHistory(string senderUID, string receiverUID);
        public Task<List<MessageDto>> GetGroupMessageHistory(string senderUID, string roomName);

        public Task UpdateMessageHistory(MessageDto newMessage);
        public Task JoinRoom(ParticipantDto participantDto);
        public Task ExitRoom(ParticipantDto participantDto);
    }
}

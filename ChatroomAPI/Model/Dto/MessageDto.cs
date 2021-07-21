using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
        public int? RoomId { get; set; }
        public int MessageTypeId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Frontend
{
    public class MessageToRoom
    {
        public string UID { get; set; }
        public string Sender { get; set; }
        public string SenderUID { get; set; }
        public int? MessageTypeId { get; set; }
        public string MessageBody { get; set; }
        public string RoomName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

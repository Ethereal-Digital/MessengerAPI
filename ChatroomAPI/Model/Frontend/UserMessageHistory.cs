using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Frontend
{
    public class UserMessageHistory
    {
        public int ItemSize { get; set; }
        public int Counter { get; set; }
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
        public string RoomName { get; set; }
    }
}

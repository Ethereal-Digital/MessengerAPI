using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Frontend
{
    public class UserMessageHistory
    {
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
    }
}

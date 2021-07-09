using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Dto
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public string UserUID { get; set; }
        public int RoomId { get; set; }
    }
}

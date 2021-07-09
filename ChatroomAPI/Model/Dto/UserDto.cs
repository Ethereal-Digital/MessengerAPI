using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UID { get; set; } 
        public string Name { get; set; }
        public string Password { get; set; }
    }
}

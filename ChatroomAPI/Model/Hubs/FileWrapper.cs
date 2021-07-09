using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Hubs
{
    public class FileWrapper
    {
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public IFormFile file { get; set; }
    }
}

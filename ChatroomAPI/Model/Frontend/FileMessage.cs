using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Frontend
{
    public class FileMessage
    {
        public IFormFile File { get; set; }
        public string MessageInfo { get; set; }
    }
}

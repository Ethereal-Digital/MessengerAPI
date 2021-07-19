using ChatroomAPI.Model.Frontend;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Services.Interface
{
    public interface IFileServices
    {
        public Task SaveFile(IFormFile file, Message message);
        public Task<(MemoryStream ms, string fileName)> DownloadFile(string attachment_id);
    }
}

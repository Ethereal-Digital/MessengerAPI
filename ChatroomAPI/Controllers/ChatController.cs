using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using ChatroomAPI.Model.Hubs;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Cors;
using ChatroomAPI.Model;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IO;
using ChatroomAPI.Services;
using ChatroomAPI.Model.Dto;
using ChatroomAPI.Services.Interface;
using ChatroomAPI.Model.Frontend;
using Newtonsoft.Json;

namespace ChatroomAPI.Controllers
{
    //[EnableCors("MyPolicy")]
    //[Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        private IChatServices _chatService { get; set; }
        private IHttpContextAccessor _hcontext;

        public ChatController(ILogger<ChatController> logger, [NotNull] IHubContext<ChatHub> chatHub, IChatServices chatService, IHttpContextAccessor haccess)
        {
            _logger = logger;
            _hubContext = chatHub;
            _chatService = chatService;
            _hcontext = haccess;

            var ddd = Directory.GetCurrentDirectory();
            var dddd = Path.GetFullPath(Path.Combine(ddd, @"..\..\"));
        }

        //[HttpPost]
        //public async Task<string> SendFile()
        //{
        //    var provider = new MultipartFormDataStreamProvider(@"C:\Users\ACER\Desktop\Jaeden\Personal\Tools\ChatApplication\FileStorage\TestUpload");
        //    await Request.Content.ReadAsMultipartAsync(provider);

        //    var myParameter = provider.FormData.GetValues("myParameter").FirstOrDefault();
        //    var count = provider.FileData.Count;

        //    return count + " / " + myParameter;
        //}

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendFile([FromForm] FileMessage fileMessage)
        {
            try
            {
                Message message = JsonConvert.DeserializeObject<Message>(fileMessage.MessageInfo);
                await _chatService.SendFileMessageToAll(fileMessage.File, message);

                return Ok(new { Size = FileServices.SizeConverter(fileMessage.File.Length) } );
                //return Ok(new { files.Count, Size = FileServices.SizeConverter(files.Sum(f => f.Length)) });
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            string filePath = @"C:\Users\ACER\Desktop\Jaeden\Personal\Tools\ChatApplication\FileStorage\TestUpload\" + fileName;

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            //var types = GetMimeTypes();
            //var ext = Path.GetExtension(filePath).ToLowerInvariant();

            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".mp3", "audio/mpeg"},
                {".wav","audio/wav" }
            };
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> SendImage(IFormFile file)
        //{
        //    long size = file.Length;

        //    if (file.Length > 0)
        //    {
        //        var filePath = Path.GetTempFileName();

        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            await file.CopyToAsync(stream);
        //        }
        //    }

        //    return Ok("Done");
        //}

        [HttpGet]
        public IActionResult GetRoomList()
        {
            var RoomList = _chatService.GetRoomList();

            return Ok(RoomList);
        }

        [HttpPost]
        public IActionResult UpdateUsersHubConnection(UserConnectionInfo UserConnectionInfo)
        {
            _chatService.UpdateUserHubConnection(UserConnectionInfo);
            _chatService.RejoinRoom(UserConnectionInfo);

            return Ok("Updated user connection.");
        }

        [HttpPost]
        public async Task<IActionResult> GetMessageHistory(UserMessageHistory userMessageHistory)
        {
            try
            {
                var message = await _chatService.GetMessageHistory(userMessageHistory);

                return Ok(message);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGroupMessageHistory(UserGroupMessageHistory userGroupMessageHistory)
        {
            try
            {
                var message = await _chatService.GetGroupMessageHistory(userGroupMessageHistory);

                return Ok(message);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            try
            {
                await _chatService.SendMessage(message);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
   
        [HttpPost]
        public async Task<IActionResult> SendMessageToAll(Message message)
        {
            try
            {
                await _chatService.SendMessageToAll(message);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToRoom(MessageToRoom message)
        {
            try
            {
                await _chatService.SendMessageToRoom(message);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> JoinRoom(UserRoomInfo userRoomInfo)
        {
            try
            {
                await _chatService.JoinRoom(userRoomInfo);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExitRoom(UserRoomInfo userRoomInfo)
        {
            try
            {
                await _chatService.ExitRoom(userRoomInfo);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }

}

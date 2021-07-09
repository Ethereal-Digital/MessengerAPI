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

        public ChatController(ILogger<ChatController> logger, [NotNull] IHubContext<ChatHub> chatHub, IChatServices ChatService, IHttpContextAccessor haccess)
        {
            _logger = logger;
            _hubContext = chatHub;
            _chatService = ChatService;
            _hcontext = haccess;
        }

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

            return Ok("Updated connection & rejoined room");
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Consumes("multipart/form-data")]
        //public async Task<HttpResponseMessage> Upload()
        //{

        //        var streamProvider = new MultipartMemoryStreamProvider();
        //        await Request.Content.ReadAsMultipartAsync(streamProvider);
        //        var fileStream = await streamProvider.Contents[0].ReadAsStreamAsync();


        //    return Ok("Done");
        //}

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

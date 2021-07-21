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
using Microsoft.AspNetCore.Hosting;
using ChatroomAPI.Middleware;

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
        private IFileServices _fileService { get; set; }
        private IChatServices _chatService { get; set; }

        public ChatController(ILogger<ChatController> logger, [NotNull] IHubContext<ChatHub> chatHub, 
            IChatServices chatService, IFileServices fileService)
        {
            _logger = logger;
            _hubContext = chatHub;
            _fileService = fileService;
            _chatService = chatService;
        }

        /// <summary>    
        /// Get the list of existing room.
        /// </summary>    
        /// <returns></returns> 
        [HttpGet]
        public async Task<IActionResult> GetRoomList()
        {
            var RoomList = await _chatService.GetRoomList();
            return Ok(RoomList);
        }

        /// <summary>
        /// Get and download relevant file through attachment id.
        /// </summary>
        /// <param name="attachment_id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string attachment_id)
        {
            try
            {
                var tupleValues = await _fileService.DownloadFile(attachment_id);
                return File(tupleValues.ms, System.Net.Mime.MediaTypeNames.Application.Octet, tupleValues.fileName);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            
        }

        /// <summary>
        /// Update and maintain user chat connection.
        /// </summary>
        /// <param name="UserConnectionInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserConnection(UserConnectionInfo UserConnectionInfo)
        {
            try
            {
                _chatService.UpdateUserHubConnection(UserConnectionInfo);
                await _chatService.RejoinRoom(UserConnectionInfo);

                return Ok("Updated user connection.");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
           
        }

        /// <summary>
        /// Get user/room message history.
        /// </summary>
        /// <param name="userMessageHistory"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Send message to user/room.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(MessageRequestFilter))]
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

        /// <summary>
        /// Send file to user/room.
        /// </summary>
        /// <param name="fileMessage"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [TypeFilter(typeof(MessageRequestFilter))]
        public async Task<IActionResult> SendFile([FromForm] FileMessage fileMessage)
        {
            try
            {
                Message message = JsonConvert.DeserializeObject<Message>(fileMessage.MessageInfo);
                await _chatService.SendFileMessage(fileMessage.File, message);

                return Ok(new { Name = fileMessage.File.FileName, Size = FileServices.SizeConverter(fileMessage.File.Length) });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// To manage the user to join or exit the room.
        /// </summary>
        /// <param name="userRoomInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RoomAction(UserRoomInfo userRoomInfo)
        {
            try
            {
                await _chatService.RoomAction(userRoomInfo);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }

}

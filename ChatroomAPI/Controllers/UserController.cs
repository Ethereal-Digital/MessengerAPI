using ChatroomAPI.Database;
using ChatroomAPI.Model;
using ChatroomAPI.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private ChatContext _chatContext { get; set; }

        public UserController(ChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        [HttpPost]
        public IActionResult Login(UserDto user)
        {
            List<UserDto> messages = _chatContext.users.ToList();

            if (messages.Where(x => x.Name.ToLower() == user.Name.ToLower()).FirstOrDefault() != null)
            {
                var selectedUser = messages.Where(x => x.Password == user.Password && x.Name.ToLower() == user.Name.ToLower()).FirstOrDefault();

                if(selectedUser != null)
                    return Ok(selectedUser);

                else
                    return BadRequest("Wrong Password.");

            }
            else
                return BadRequest("User not exist.");
                
        }
    }

}

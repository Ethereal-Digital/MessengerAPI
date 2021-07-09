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
        string Filejson = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"\MockDB\Users.json");

        [HttpPost]
        public IActionResult Login(UserDto user)
        {
            List<UserDto> messages = JsonConvert.DeserializeObject<List<UserDto>>(Filejson);

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

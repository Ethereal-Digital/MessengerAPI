using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavascriptClient1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LogoutController : ControllerBase
    {
        public async Task Logout()
        {
            try
            {
                await HttpContext.SignOutAsync("ChatCookie");

                await HttpContext.ChallengeAsync("ChatCookie", new AuthenticationProperties
                {
                    RedirectUri = "/"
                });

            }
            catch
            {
                throw new Exception("Unable to Logout, try refresh browser.");
            }
        }
    }
}

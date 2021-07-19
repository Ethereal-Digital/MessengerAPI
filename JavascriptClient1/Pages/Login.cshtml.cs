using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace JavascriptClient1.Pages
{
    public class LoginModel : PageModel
    {
        //private string ApiBaseURL = "https://localhost:44364";
        private string ApiBaseURL = "https://cece116c57cc.ngrok.io";

        private static string _notification { get; set; }
        public string notification
        {
            get
            {
                if (_notification == null)
                    _notification = "";

                return _notification;
            }
        }

        public void OnGet()
        {
            //var user = User as ClaimsPrincipal;
            //var identity = user.Identity as ClaimsIdentity;

            //foreach(var claim in identity.Claims)
            //{
            //    identity.RemoveClaim(claim);
            //}
        }

        public async Task<IActionResult> OnPostLogin()
        {
            User user = new User();

            var name = Request.Form["username"];
            var pass = Request.Form["password"];
            user.Name = name;
            user.Password = pass;

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync($@"{ApiBaseURL}/user/Login", data);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    user = new User();
                    var claims = new List<Claim>();

                    user = JsonConvert.DeserializeObject<User>(result);
                    //claims.Add(new Claim(ClaimTypes.Name, user.name));
                    claims.Add(new Claim("Username", user.Name));
                    claims.Add(new Claim("UserID", user.UID));

                    var claimsIdentity = new ClaimsIdentity(claims, "ChatCookie");
                    await HttpContext.SignInAsync("ChatCookie", new ClaimsPrincipal(claimsIdentity));

                    _notification = null;

                    return Redirect("/Index");
                }
                else
                {
                    _notification = result;

                    return Redirect("/Login");
                }
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}

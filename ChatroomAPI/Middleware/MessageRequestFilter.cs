using ChatroomAPI.Model.Frontend;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Middleware
{
    public class MessageRequestFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Message message = new Message();

            // Get payload
            Message messagePayload = (Message)context.ActionArguments?.Values.FirstOrDefault(v => v is Message);
            FileMessage fileMessagePayload = (FileMessage)context.ActionArguments?.Values.FirstOrDefault(v => v is FileMessage);

            if(fileMessagePayload != null)
            {
                message = JsonConvert.DeserializeObject<Message>(fileMessagePayload.MessageInfo);
                message.UID = Guid.NewGuid().ToString();

                string json = JsonConvert.SerializeObject(message);
                fileMessagePayload.MessageInfo = json;
            }
            else 
            {
                messagePayload.UID = Guid.NewGuid().ToString();
            }
          
            await next();
        }
    }
}

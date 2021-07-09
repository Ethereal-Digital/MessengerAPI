using ChatroomAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Middleware.Interface
{
    public interface IChatMiddleware
    {
        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo);

        public string GetUserHubConnectionId(string userUID);
        public UserConnectionInfo GetUserInformation(string userUID);
        
    }
}

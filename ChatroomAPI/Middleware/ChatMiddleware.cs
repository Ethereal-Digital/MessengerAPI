using ChatroomAPI.Middleware.Interface;
using ChatroomAPI.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Middleware
{
    public sealed class ChatMiddleware : IChatMiddleware
    {
        private static readonly object _lock = new object ();
        private static ConcurrentDictionary<string, UserConnectionInfo> _onlineUsers { get; set; }
        public static ConcurrentDictionary<string, UserConnectionInfo> OnlineUsers
        {
            get
            {
                if(_onlineUsers == null)
                {
                    lock (_lock)
                    {
                        if (_onlineUsers == null)
                            _onlineUsers = new ConcurrentDictionary<string, UserConnectionInfo>();
                    }
                }

                return _onlineUsers;
            }

        }

        private static readonly object _lock1 = new object();
        private static ConcurrentDictionary<string, string> _usersGroup { get; set; }
        private static ConcurrentDictionary<string, string> UsersGroup
        {
            get
            {
                if(_usersGroup == null)
                {
                    lock (_lock1)
                    {
                        if (_usersGroup == null)
                            _usersGroup = new ConcurrentDictionary<string, string>();
                    }
                }

                return _usersGroup;
            }
        }

        private void UpdateUserGroup()
        {

        }

        public string GetUserHubConnectionId(string userUID)
        {
            string connectionID = _onlineUsers.Where(x => x.Value.UserUID == userUID).Select(x => x.Value.ConnectionId).FirstOrDefault();
            return connectionID;
        }

        public UserConnectionInfo GetUserInformation(string userUID)
        {
            var userInformation = _onlineUsers.Where(x => x.Value.UserUID == userUID).Select(x => x.Value).FirstOrDefault();
            return userInformation;
        }

        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo)
        {
            UserConnectionInfo oldUserInfo = new UserConnectionInfo();

            if (!OnlineUsers.TryGetValue(newUserInfo.UserUID, out oldUserInfo))
            {
                _onlineUsers.TryAdd(newUserInfo.UserUID, newUserInfo);
            }
            else
            {
                lock (oldUserInfo)
                {
                    _onlineUsers.TryUpdate(newUserInfo.UserUID, newUserInfo, oldUserInfo);
                }
            }
        }
    }
}

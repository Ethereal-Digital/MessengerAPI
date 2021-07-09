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
        private static ConcurrentDictionary<string, UserConnectionInfo> _usersHubConnection { get; set; }
        private static ConcurrentDictionary<string, UserConnectionInfo> UsersHubConnection
        {
            get
            {
                lock (_lock)
                    {
                        if (_usersHubConnection == null)
                            _usersHubConnection = new ConcurrentDictionary<string, UserConnectionInfo>();
                    }

                return _usersHubConnection;
            }

        }

        private static readonly object _lock1 = new object();
        private static ConcurrentDictionary<string, string> _usersGroup { get; set; }
        private static ConcurrentDictionary<string, string> UsersGroup
        {
            get
            {
                lock (_lock1)
                {
                    if (_usersGroup == null)
                        _usersGroup = new ConcurrentDictionary<string, string>();
                }

                return _usersGroup;
            }
        }

        private void UpdateUserGroup()
        {

        }

        public string GetUserHubConnectionId(string userUID)
        {
            string connectionID = UsersHubConnection.Where(x => x.Value.UserUID == userUID).Select(x => x.Value.ConnectionId).FirstOrDefault();
            return connectionID;
        }

        public UserConnectionInfo GetUserInformation(string userUID)
        {
            var userInformation = UsersHubConnection.Where(x => x.Value.UserUID == userUID).Select(x => x.Value).FirstOrDefault();
            return userInformation;
        }

        public void UpdateUserHubConnection(UserConnectionInfo newUserInfo)
        {
            UserConnectionInfo oldUserInfo = new UserConnectionInfo();

            if (!UsersHubConnection.TryGetValue(newUserInfo.UserUID, out oldUserInfo))
            {
                UsersHubConnection.TryAdd(newUserInfo.UserUID, newUserInfo);
            }
            else
            {
                lock (oldUserInfo)
                {
                    UsersHubConnection.TryUpdate(newUserInfo.UserUID, newUserInfo, oldUserInfo);
                }
            }
        }
    }
}

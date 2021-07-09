using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Model.Hubs
{
    public interface IStronglyTypeHub
    {
        Task NewMessage(string message);
    }
}

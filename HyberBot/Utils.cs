using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public static class Utils
    {
        public static bool IsHydra(this SocketUser user)
        {
            return user.Username == "hydraxous";
        }

        public static string GetDisplayName(this SocketUser user)
        {
            if (user.GlobalName == null)
                return user.Username;

            return user.GlobalName;
        }
    }
}

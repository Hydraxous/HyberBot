using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions
{
    [Serializable]
    public abstract class MatchingPattern
    {
        public abstract bool CheckMatch(SocketMessage message);
    }
}

using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.ReactionMethods
{
    public abstract class ReactionMethod
    {
        public virtual async Task React(SocketMessage message)
        {
            await Task.CompletedTask;
        }
    }
}

using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.ReactionMethods
{
    public class EmoteReactionMethod : ReactionMethod
    {

        private IEmote emote;

        public EmoteReactionMethod(IEmote emote) 
        {
            this.emote = emote;
        }

        public override async Task React(SocketMessage message)
        {
            await message.AddReactionAsync(emote);
        }

    }
}

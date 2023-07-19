using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.ReactionMethods
{
    [Serializable]
    public class EmojiReactionMethod : ReactionMethod
    {
        public string Name { get; }

        private Emoji emoji;

        public EmojiReactionMethod(Emoji emoji)
        {
            this.emoji = emoji;
            Name = emoji.Name;
        }

        public EmojiReactionMethod()
        {

        }

        public override async Task React(SocketMessage message)
        {
            await message.AddReactionAsync(emoji);
        }
    }
}

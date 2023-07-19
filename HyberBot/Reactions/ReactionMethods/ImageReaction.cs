using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.ReactionMethods
{
    [Serializable]
    public class ImageReaction : ReactionMethod
    {

        public string ImageURL { get; set; }

        public ImageReaction(string imageURL) 
        {
            this.ImageURL = imageURL;
        }


        public override async Task React(SocketMessage message)
        {
            string msgText = message.Content;
            msgText = "\""+msgText+"\"";
            msgText += "\n"+ImageURL;
            await message.Channel.SendMessageAsync(msgText);
        }
    }
}

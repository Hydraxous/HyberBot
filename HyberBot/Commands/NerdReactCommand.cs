using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Commands
{
    public class NerdReactCommand
    {
        DiscordSocketClient client;

        public NerdReactCommand(DiscordSocketClient client) 
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {
            client.MessageReceived += HandleMessage;

            await Task.CompletedTask;
        }

        private Dictionary<ulong,string> lastMessages = new Dictionary<ulong,string>();

        private async Task HandleMessage(SocketMessage message)
        {
            if (message.Author.IsBot)
                return;

            if(message.Content.ToLower().Contains("nerd react him"))
            {
                if(lastMessages.ContainsKey(message.Channel.Id))
                {
                    await NerdReactLastMessageInChannel(message.Channel);
                    await Task.CompletedTask;
                    return;
                }
            }

            if(!lastMessages.ContainsKey(message.Channel.Id))
                lastMessages.Add(message.Channel.Id, message.Content);

            lastMessages[message.Channel.Id] = message.Content;
            await Task.CompletedTask;
        }


        private async Task NerdReactLastMessageInChannel(ISocketMessageChannel channel)
        {
            SocketChannel chan = channel as SocketChannel;

            if(chan == null)
            {
                await Task.CompletedTask;
                return;
            }

            string lastMessageContent = lastMessages[channel.Id];

            string finalMessage = $"*\"{lastMessageContent}\"*\nhttps://media.tenor.com/SRX8X6DNF6QAAAAd/nerd-nerd-emoji.gif";
            await channel.SendMessageAsync(finalMessage);
        }
        

    }
}

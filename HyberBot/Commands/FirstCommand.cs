using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Commands
{
    public class FirstCommand
    {

        private DiscordSocketClient client;

        public FirstCommand(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {

            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("hello-world");
            globalCommand.WithDescription("This is my first command!");

            try
            {
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            } catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }
    }
}

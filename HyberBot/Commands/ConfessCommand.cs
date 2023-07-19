using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Commands
{
    public class ConfessCommand
    {
        private DiscordSocketClient client;

        public ConfessCommand(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {

            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("confess");
            globalCommand.WithDescription("Speak anonymously");
            globalCommand.AddOption("message", ApplicationCommandOptionType.String, "message to confess", isRequired:true);

            try
            {
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                client.SlashCommandExecuted += HandleCommand;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private async Task HandleCommand(SocketSlashCommand command)
        {
            if (command.Data.Name != "confess")
            {
                await Task.CompletedTask;
                return;
            }

            string confession = (string) command.Data.Options.First().Value;

            var message = await command.Channel.SendMessageAsync(confession);

            await command.RespondAsync("speaking anon-e-mouse lee", ephemeral:true);
        }
    }
}

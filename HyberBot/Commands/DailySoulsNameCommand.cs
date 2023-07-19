using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using HyberBot.DailyName;

namespace HyberBot.Commands
{
    public class DailySoulsNameCommand
    {
        private DiscordSocketClient client;
        
        public DailySoulsNameCommand(DiscordSocketClient client) 
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {
            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("souls-rename");
            globalCommand.WithDescription("Changes your name to a soulsname over time");
            globalCommand.AddOption("enabled", ApplicationCommandOptionType.Boolean, "Set the registration for yourself", isRequired:true);
            globalCommand.AddOption(new SlashCommandOptionBuilder()
                .WithName("interval")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithDescription("Interval at which to change your name.")
                .WithRequired(true)
                .AddChoice("Every Minute", 58)
                .AddChoice("Hourly", 3600)
                .AddChoice("Daily", 86400)
                .AddChoice("Weekly", 604800)
                .AddChoice("Monthly", 2419200));

            try
            {
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                client.SlashCommandExecuted += InterpretCommand;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public async Task InterpretCommand(SocketSlashCommand command)
        {
            if(command.Data.Name != "souls-rename")
            {
                await Task.CompletedTask;
                return;
            }

            if(command.GuildId == null)
            {
                await command.RespondAsync("You need to execute this command in a server, smart guy....");
                return;
            }

            try
            {
                bool isEnabled = (bool)command.Data.Options.First().Value;
                long timeRequired = (long)command.Data.Options.ElementAt(1).Value;

                ulong guildID = command.GuildId.Value;
                ulong userID = command.User.Id;

                if(!isEnabled)
                {
                    DailyNameController.RemoveRecord(guildID, userID);
                    await command.RespondAsync("Soulnaming is now disabled for you. :((", ephemeral:true);
                    return;
                }

                DailyNameController.AddRecord(guildID, userID, timeRequired);
                await command.RespondAsync("YOUR JUDGEMENT IS GREAT MY LORD!", ephemeral:true);
            }
            catch(Exception ex)
            {  
                Logger.LogError(ex);
                await command.RespondAsync(">m< whoops! something went wrong!");
            }
        }

    }
}

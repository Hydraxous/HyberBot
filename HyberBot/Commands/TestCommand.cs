using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using HyberBot.DataPersistence;
using HyberBot.DataPersistence.DataFileTypes;
using Newtonsoft.Json;

namespace HyberBot.Commands
{
    public class TestCommand
    {
        private DiscordSocketClient client;

        public TestCommand(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {

            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("test");
            globalCommand.WithDescription("debugging command");
            globalCommand.AddOption("subcommand", ApplicationCommandOptionType.String, "Subcommand", isRequired: true);
            globalCommand.AddOption("data", ApplicationCommandOptionType.String, "dataaa", isRequired: true);

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

            if(command.Data.Name != "test")
            {
                await Task.CompletedTask;
                return;
            }

            if(command.User.Username != "hydraxous" || command.GuildId == null)
            {
                await command.RespondAsync($"I'm sorry {command.User.GetDisplayName()}, I'm afraid I can't do that.");
                return;
            }

            string subcommand = (string) command.Data.Options.First().Value;
            string data = "";

            if(command.Data.Options.Count > 1)
            {
                data = (string)command.Data.Options.ElementAt(1).Value;
            }
        
            await InterpredSubcommand(subcommand, data, command);
        }

        private async Task InterpredSubcommand(string commandName, string data, SocketSlashCommand command)
        {
            if(string.IsNullOrEmpty(commandName))
            {
                await command.RespondAsync("No data?");
                return;
            }

            GuildData dataFile = null;

            switch (commandName)
            {
                case "write":
                    dataFile = GuildDataManager.GetGuildData<GuildData>(command.GuildId.Value, "basicData.txt");
                    GuildDataManager.SaveGuildData<GuildData>(command.GuildId.Value, "basicData.txt", dataFile);
                    await command.RespondAsync("Saved data", ephemeral: true);
                    break;

                case "read":
                    dataFile = GuildDataManager.GetGuildData<GuildData>(command.GuildId.Value, "basicData.txt");
                    await command.RespondAsync($"Your data is {dataFile.dataMessage}", ephemeral: true);
                    break;

                case "set":
                    dataFile = GuildDataManager.GetGuildData<GuildData>(command.GuildId.Value, "basicData.txt");
                    dataFile.dataMessage = data;
                    await command.RespondAsync($"Set data to {data}", ephemeral: true);
                    break;

                case "reload":
                    bool reloadComplete = GuildDataManager.ForceReloadFile<GuildData>(command.GuildId.Value, "basicData.txt");

                    string reply = (reloadComplete) ? "Reloaded file successfully." : "Failed to reload file. See console output.";
                    await command.RespondAsync(reply, ephemeral: true);
                    break;

                case "clear-cache":
                    GuildDataManager.ClearCache();
                    await command.RespondAsync($"Data cache cleared.", ephemeral: true);
                    break;

                case "writenames":

                    IntruderNames names = new IntruderNames();

                    string[] allnames = names.names;
                    string json = JsonConvert.SerializeObject(allnames);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "soulsnames.txt");
                    File.WriteAllText(filePath, json);
                    await command.RespondAsync("Done.");
                    break;
            }
        }
    }
}

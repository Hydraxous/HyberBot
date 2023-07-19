using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Commands
{
    public class SoulsNameCommand
    {
        DiscordSocketClient client;

        public SoulsNameCommand(DiscordSocketClient client) 
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {
            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("soulsname");
            globalCommand.WithDescription("Generate a souls name");

            SlashCommandOptionBuilder modifySubCommandGroup = await BuildModifySubCommandGroup();
            globalCommand.AddOption(modifySubCommandGroup);

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

        private async Task<SlashCommandOptionBuilder> BuildModifySubCommandGroup()
        {
            SlashCommandOptionBuilder modifyCommandGroup = new SlashCommandOptionBuilder();
            modifyCommandGroup.Name = "name";
            modifyCommandGroup.Description = "Modify name list";
            modifyCommandGroup.IsRequired = false;
            modifyCommandGroup.Type = ApplicationCommandOptionType.SubCommandGroup;

            SlashCommandOptionBuilder addCommand = await BuildAddSubcommand();
            modifyCommandGroup.AddOption(addCommand);

            SlashCommandOptionBuilder getCommand = await BuildGetSubcommand();
            modifyCommandGroup.AddOption(getCommand);


            return modifyCommandGroup;
        }

        private async Task<SlashCommandOptionBuilder> BuildAddSubcommand()
        {
            return new SlashCommandOptionBuilder()
                .WithName("add")
                .WithDescription("Add a new name to the list")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("name", ApplicationCommandOptionType.String, "Name to add", isRequired: true);
        }

        private async Task<SlashCommandOptionBuilder> BuildGetSubcommand()
        {
            return new SlashCommandOptionBuilder()
                .WithName("get")
                .WithDescription("Get a souls name!")
                .WithType(ApplicationCommandOptionType.SubCommand);
        }

        private async Task HandleCommand(SocketSlashCommand command)
        {
            if (command.Data.Name != "soulsname")
            {
                await Task.CompletedTask;
                return;
            }

            if(command.Data.Options.Count > 0)
            {
                await HandleSubcommand(command);
                return;
            }  
        }

        private async Task HandleSubcommand(SocketSlashCommand command)
        {
            string subcommandName = command.Data.Options.First().Name;
            
            switch(subcommandName)
            {
                case "name":
                    await HandleSubCommand(command);
                    break;

                default:
                    await command.RespondAsync("Invalid subcommand", ephemeral:true);
                    break;
            }
        }

        private async Task RespondWithSoulsName(SocketSlashCommand command)
        {
            try
            {
                string name = IntruderNamer.GetName(RandomExtensions.BinaryRandom());
                string preMessage = "Your souls name is.....\n";

                preMessage += "***" + name + "***";
                await command.RespondAsync(preMessage);
            }
            catch (Exception ex)
            {
                string output = "Erm execution issue!?";

                Logger.LogError(ex);

                if (command.User.IsHydra())
                    output += $"\n{ex}";

                await command.RespondAsync(output, ephemeral: true);
            }
        }

        private async Task HandleSubCommand(SocketSlashCommand command)
        {
            string subcommandName = command.Data.Options.First().Options.First().Name;

            switch(subcommandName)
            {
                case "add":
                    await HandleAddCommand(command);
                    break;

                case "get":
                    await RespondWithSoulsName(command);
                    break;

                default:
                    await command.RespondAsync("Invalid subcommand");
                    break;
            }
        }

        private async Task HandleAddCommand(SocketSlashCommand command)
        {

            string newname = (string)command.Data.Options.First().Options.First().Options.First().Value;

            if (string.IsNullOrEmpty(newname))
            {
                await command.RespondAsync("You have to actually type something...");
                return;
            }

            if (IntruderNamer.NameListContains(newname))
            {
                await command.RespondAsync("That name is already a souls name. Try another.");
                return;
            }

            IntruderNamer.AddNewName(newname);
            await command.RespondAsync($"***{newname}*** is now a souls name!");
        }
    }
}

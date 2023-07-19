using Discord.WebSocket;
using HyberBot.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using HyberBot.Reactions.ReactionMethods;
using Discord.Rest;
using HyberBot.Reactions.Matchers;
using System.Data;

namespace HyberBot.Commands
{
    public class CreateReactionCommand
    {
        private DiscordSocketClient client;
        private Reactor reactor;

        public CreateReactionCommand(DiscordSocketClient client, Reactor reactor)
        {
            this.client = client;
            this.reactor = reactor;
        }

        public async Task BuildCommand()
        {

            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("add-reaction");
            globalCommand.WithDescription("Adds a reaction whenever a phrase is said");

            globalCommand.AddOption("id", ApplicationCommandOptionType.String, "A unique id for the reaction", isRequired: true);
            globalCommand.AddOption("phrase", ApplicationCommandOptionType.String, "String to match", isRequired: true);
            globalCommand.AddOption("emoji", ApplicationCommandOptionType.String, "Unicode of emoji to react with example \\U0001F408 is cat emoji", isRequired: true);

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
            if (command.Data.Name != "add-reaction")
            {
                await Task.CompletedTask;
                return;
            }

            try
            {
                Dictionary<string, string> optionValues = new Dictionary<string, string>
                {
                    { "id", ""},
                    { "phrase", ""},
                    { "emoji", ""}
                };

                try
                {
                    if (command.Data.Options.Count < 3)
                    {
                        throw new ArgumentException("Not enough arguments");
                    }

                    foreach(var option in command.Data.Options)
                    {
                        if(!optionValues.ContainsKey(option.Name))
                        {
                            throw new ArgumentException("invalid arg id");
                        }

                        optionValues[option.Name] = (string) option.Value;
                    }

                    foreach(KeyValuePair<string, string> pair in optionValues)
                    {
                        if(string.IsNullOrEmpty(pair.Value))
                        {
                            throw new ArgumentException("Missing required data!!!");
                        }
                    }

                    IEmote emote = null;

                    if (Emoji.TryParse(optionValues["emoji"], out Emoji result))
                    {
                        emote = result;
                    }else if(Emote.TryParse(optionValues["emoji"], out Emote result2))
                    {
                        throw new ArgumentException("Custom Emotes are not supported at this time.");
                        emote = result2;
                    }else
                    {
                        throw new ArgumentException("Invalid emoji or emote.");
                    }


                    EmoteReactionMethod emoteReaction = new EmoteReactionMethod(result);
                    MatchStringInGuild stringMatcher = new MatchStringInGuild(optionValues["phrase"], command.GuildId.Value);
                    Reaction reaction = new Reaction(optionValues["id"],emoteReaction, stringMatcher);
                    
                    if(reactor.ContainsReactionByName(reaction.Name))
                    {
                        throw new DuplicateNameException($"{optionValues["id"]} already exists.");
                    }

                    reactor.AddReaction(reaction);

                    string confirmMessage = $"Added reaction {optionValues["id"]} successfully.";

                    await command.RespondAsync(confirmMessage, ephemeral:true);

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
            catch (Exception ex)
            {
                string output = "Erm execution issue!?";
               
                Logger.LogError(ex);

                if (command.User.IsHydra())
                    output += $"\n{ex}";

                await command.RespondAsync(output, ephemeral: true);
            }
        }
    }
}

using Discord;
using Discord.WebSocket;
using HyberBot.KnightsTryIntegration.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.KnightsTryIntegration
{
    public class KnightTryCommand
    {
        private DiscordSocketClient discord;

        private KnightClient knight;

        public KnightTryCommand(DiscordSocketClient client)
        {
            this.discord = client;
            knight = new KnightClient();

        }

        public async Task BuildCommand()
        {
            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("kt");
            globalCommand.WithDescription("Knights try commands");
            globalCommand.AddOption(await BuildActSubCommand());
            globalCommand.AddOption(await BuildClientSubCommands());

            try
            {
                await discord.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                discord.SlashCommandExecuted += HandleCommand;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private async Task<SlashCommandOptionBuilder> BuildActSubCommand()
        {

            SlashCommandOptionBuilder actSubCommandGroup = new SlashCommandOptionBuilder();
            actSubCommandGroup.Name = "act";
            actSubCommandGroup.Description = "perform in-game actions";
            actSubCommandGroup.Type = ApplicationCommandOptionType.SubCommandGroup;


            SlashCommandOptionBuilder killSubCommand = new SlashCommandOptionBuilder();
            killSubCommand.Name = "rtd";
            killSubCommand.Description = "roll the dice";
            killSubCommand.Type = ApplicationCommandOptionType.SubCommand;

            actSubCommandGroup.AddOption(killSubCommand);

            return actSubCommandGroup;
        }

        private async Task<SlashCommandOptionBuilder> BuildClientSubCommands()
        {
            SlashCommandOptionBuilder clientSubCommandGroup = new SlashCommandOptionBuilder();
            clientSubCommandGroup.Name = "client";
            clientSubCommandGroup.Description = "controls connection state";
            clientSubCommandGroup.Type = ApplicationCommandOptionType.SubCommandGroup;


            SlashCommandOptionBuilder connectSubCommand = new SlashCommandOptionBuilder();
            connectSubCommand.Name = "connect";
            connectSubCommand.Description = "Connect to a session";
            connectSubCommand.Type = ApplicationCommandOptionType.SubCommand;
            connectSubCommand.AddOption("ip", ApplicationCommandOptionType.String, "Ip to connect to", isRequired: true);
            connectSubCommand.AddOption("port", ApplicationCommandOptionType.Integer, "port to connect", isRequired: true);

            clientSubCommandGroup.AddOption(connectSubCommand);

            SlashCommandOptionBuilder disconnectSubCommand = new SlashCommandOptionBuilder();
            disconnectSubCommand.Name = "disconnect";
            disconnectSubCommand.Description = "disconnect from current session";
            disconnectSubCommand.Type = ApplicationCommandOptionType.SubCommand;

            clientSubCommandGroup.AddOption(disconnectSubCommand);

            SlashCommandOptionBuilder subscribeChannelEvents = new SlashCommandOptionBuilder();
            subscribeChannelEvents.Name = "listener";
            subscribeChannelEvents.Description = "starts listener and logs received events";
            subscribeChannelEvents.Type = ApplicationCommandOptionType.SubCommand;

            clientSubCommandGroup.AddOption(subscribeChannelEvents);

            return clientSubCommandGroup;

        }

        private async Task HandleCommand(SocketSlashCommand command)
        {
            if (command.Data.Name != "kt")
                return;

            if (command.Data.Options.Count == 0)
            {
                await command.RespondAsync("You didnt provide arguments dummy");
                return;
            }


            try
            {
                await HandlSubCommand(command);

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

        private async Task HandlSubCommand(SocketSlashCommand command)
        {
            string subCommandGroupName = command.Data.Options.First().Name;

            switch (subCommandGroupName)
            {
                case "client":
                    await ConnectSubCommand(command);
                    break;

                case "act":
                    await ActSubCommand(command);
                    break;
            }

        }

        private async Task ConnectSubCommand(SocketSlashCommand command)
        {

            string subcommand = command.Data.Options.First().Options.First().Name;

            switch(subcommand)
            {
                case "connect":

                    var ip = (string)command.Data.Options.First().Options.First().Options.ElementAt(0).Value;
                    long portLong = (long)command.Data.Options.First().Options.First().Options.ElementAt(1).Value;
                    int port = Convert.ToInt32(portLong);

                    await knight.ConnectAsync(ip, port);
                    await command.RespondAsync("Connected Successfully!", ephemeral:true);
                    break;


                case "disconnect":
                    knight.Disconnect();
                    await command.RespondAsync("Disconnected!", ephemeral:true);
                    break;

                case "listener":
                    if (!knight.StartCommandListener())
                    {
                        await command.RespondAsync("Connection is not active. Listener failed to start.", ephemeral: true);
                    }else
                    {
                        if(command.GuildId == null || command.ChannelId == null)
                        {
                            await command.RespondAsync("Guild ID or Channel ID is null!", ephemeral:true);
                            return;
                        }

                        SubscribeChannelToClientEvents(command.GuildId.Value, command.ChannelId.Value, knight);
                        await command.RespondAsync("Attempting To Start Listener", ephemeral:true);
                    }
                    break;
            }

        }

        private void SubscribeChannelToClientEvents(ulong guild, ulong channel, KnightClient client)
        {

            ChannelServerEventSubscription channelServerEvent = new ChannelServerEventSubscription();
            channelServerEvent.ChannelID = channel;
            channelServerEvent.GuildID = guild;
            channelServerEvent.client = client;
            channelServerEvent.CheckConnected = () => 
            {
                if (client == null)
                    return false;

                return client.Connected;
            };

            Task.Run(() => ChannelMessageListener(channelServerEvent));
        }

        private async Task ChannelMessageListener(ChannelServerEventSubscription subscription)
        {
            Queue<string> messageQueue = new Queue<string>();

            subscription.client.OnCommandReceived += EnqueueMessage;

            await discord.GetGuild(subscription.GuildID).GetTextChannel(subscription.ChannelID).SendMessageAsync("Listener started in this channel!");

            while (subscription.CheckConnected())
            {
                if(messageQueue.Count > 0)
                {
                    string message = messageQueue.Dequeue();
                    await discord.GetGuild(subscription.GuildID).GetTextChannel(subscription.ChannelID).SendMessageAsync(message);
                }

                await Task.Yield();
            }

            if(subscription.client.OnCommandReceived != null)
                subscription.client.OnCommandReceived -= EnqueueMessage;

            await discord.GetGuild(subscription.GuildID).GetTextChannel(subscription.ChannelID).SendMessageAsync("Listener stopped!");

            void EnqueueMessage(string message)
            {
                messageQueue.Enqueue(message);
            }
        }

        public struct ChannelServerEventSubscription
        {
            public Func<bool> CheckConnected;
            public ulong GuildID;
            public ulong ChannelID;
            public KnightClient client;
        }

        private async Task ActSubCommand(SocketSlashCommand command)
        {
            string subcommand = command.Data.Options.First().Options.First().Name;
            knight.SendCommandRaw(subcommand);
            await command.RespondAsync($"Action {subcommand} Sent!", ephemeral: true);
        }
    }       
}

using Discord;
using Discord.WebSocket;
using HyberBot.Commands;
using HyberBot.KnightsTryIntegration;
using HyberBot.NeatStuff;
using HyberBot.DataPersistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public class Hyber
    {
        public DiscordSocketClient Client { get; private set; }

        private HyberTimedStuff timedThings;
        private DailyAutoName autoNamer;
        private HyberReactions reactions;
        private Commander commander;

        private DiscordSocketConfig config;

        string token;

        public async Task Login()
        {
            config = new DiscordSocketConfig();
            config.GatewayIntents = GatewayIntents.All;

            Client = new DiscordSocketClient(config);
            reactions = new HyberReactions(this);
            autoNamer = new DailyAutoName(Client,60000);
            timedThings = new HyberTimedStuff(this);

            AsyncTokenOperationResult result = await TokenRetriever.RetrieveTokenAsync();
            
            if(!result.success)
            {
                Logger.LogError("Token could not be retrieved.");
                return;
            }

            token = result.token;

            await RegisterEvents();
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task RegisterEvents()
        {
            Client.Log += Logger.DLogAsync;
            Client.Ready += BuildCommands;
            Client.SlashCommandExecuted += InterpretCommand;
            autoNamer.Start();
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            var emote = Emote.Parse("<a:kittyclap:810409203436486707>");

            if (emote == null)
            {
                await Task.CompletedTask;
                return;
            }

            await arg.AddReactionAsync(emote);
        }

        private async Task InterpretCommand(SocketSlashCommand command)
        {
            Logger.Log(command.Data.Name);
            Logger.Log(command.Data.Options.Count);
        }

        private async Task BuildCommands()
        {
            FirstCommand first = new FirstCommand(Client);
            await first.BuildCommand();

            Logger.Log("Built first command");

            WhenIsCommand whenIs = new WhenIsCommand(Client);
            await whenIs.BuildCommand();

            Logger.Log("Built WhenIs");

            SoulsNameCommand soulsName = new SoulsNameCommand(Client);
            await soulsName.BuildCommand();

            Logger.Log("Built Soulsname");

            CreateReactionCommand createReaction = new CreateReactionCommand(Client, reactions.Reactor);
            await createReaction.BuildCommand();

            Logger.Log("Built add-reaction");

            KnightTryCommand ktCommand = new KnightTryCommand(Client);
            await ktCommand.BuildCommand();

            Logger.Log("Built kt");

            CatboyCommand catBoyCommand = new CatboyCommand(Client);
            await catBoyCommand.BuildCommand();

            Logger.Log("Built catboy");

            ConfessCommand confess = new ConfessCommand(Client);
            await confess.BuildCommand();
            Logger.Log("Built confess");

            NerdReactCommand nerdCommand = new NerdReactCommand(Client);
            await nerdCommand.BuildCommand();
            Logger.Log("Built nerd react");


            TestCommand testCmd = new TestCommand(Client);
            await testCmd.BuildCommand();
            Logger.Log("Built test command");

            //DailySoulsNameCommand dailySouls = new DailySoulsNameCommand(Client);
            //await dailySouls.BuildCommand();
            Logger.Log("Built souls rename");

        }
    }
}

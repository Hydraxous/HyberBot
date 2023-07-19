using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HyberBot.Commands
{

    //API endpoint https://api.catboys.com/img

    public class CatboyCommand
    {
        public const string CATBOY_API_ENDPOINT = "https://api.catboys.com/img";


        private DiscordSocketClient client;

        public CatboyCommand(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {

            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("catboy");
            globalCommand.WithDescription("Summons an image of a catboy. Very useful I know.");

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
            if (command.Data.Name != "catboy")
            {
                await Task.CompletedTask;
                return;
            }

            string catBoyUrl = await GetCatboyImage();


            var message = await command.Channel.SendMessageAsync(catBoyUrl);

            Emoji[] cats = new Emoji[2];
            cats[0] = Emoji.Parse("\U0001F63B");
            cats[1] = Emoji.Parse("\U0001F63E");

            await message.AddReactionsAsync(cats);
            await command.RespondAsync("Your catboy, as requested... :3");
        }

        private async Task<string> GetCatboyImage()
        {
            WebRequest request = WebRequest.Create(CATBOY_API_ENDPOINT) as HttpWebRequest;

            if(request == null)
            {
                return "Catboy failed to be summoned :pensive:";
            }

            request.ContentType = "application/json";

            string url = string.Empty;

            try
            {
                using (var s = request.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var json = sr.ReadToEnd();
                        dynamic parsedJson = JsonConvert.DeserializeObject<dynamic>(json);

                        url = parsedJson.url;
                    }
                }

                return url;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return "Catboy failed to be summoned :pensive:";
        }
    }
}

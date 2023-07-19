using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace HyberBot.Commands
{
    public class WhenIsCommand
    {
        DiscordSocketClient client;

        private const string IMAGE_YEARS = "years.jpg";
        private const string IMAGE_MONTHS = "months.jpg";
        private const string IMAGE_WEEKS = "weeks.jpg";
        private const string IMAGE_DAYS = "days.jpg";
        private const string IMAGE_HOURS = "hours.jpg";
        private const string IMAGE_MINUTES = "minutes.jpg";
        private const string IMAGE_SECONDS = "seconds.jpg";
        private const string IMAGE_MILLISECONDS = "milliseconds.jpg";
        private const string IMAGE_NEVER = "never.jpg";
        private const string IMAGE_PAST = "past.jpg";

        public WhenIsCommand(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task BuildCommand()
        {
            SlashCommandBuilder globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("whenis");
            globalCommand.WithDescription("Get an accurate description of when a date is");

            globalCommand.AddOption("year", ApplicationCommandOptionType.Integer, "yyyy", isRequired: true);
            globalCommand.AddOption("month", ApplicationCommandOptionType.Integer, "mm", isRequired: true);
            globalCommand.AddOption("day", ApplicationCommandOptionType.Integer, "dd", isRequired: true);
            globalCommand.AddOption("hour", ApplicationCommandOptionType.Integer, "hh (0-23)", isRequired: false);
            globalCommand.AddOption("minute", ApplicationCommandOptionType.Integer, "mm (0-59)", isRequired: false);
            globalCommand.AddOption("second", ApplicationCommandOptionType.Integer, "ss (0-59)", isRequired: false);

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
            if(command.Data.Name != "whenis")
            {
                await Task.CompletedTask;
                return;
            }

            try
            {
                int[] times = new int[6] { 0, 0, 0, 0, 0, 0 };

                for (int i = 0; i < command.Data.Options.Count; i++)
                {

                    long receivedValue = (long)command.Data.Options.ElementAt(i).Value;

                    int value = Convert.ToInt32(receivedValue);

                    if (value < 0)
                        throw new ArgumentException("Invalid date. No date can be negative you dummy.");

                    switch (i)
                    {
                        case 1:
                            if (value > 12)
                                throw new ArgumentException("Invalid month.");
                            break;
                        case 2:
                            if (value > 31)
                                throw new ArgumentException("Invalid day.");
                            break;
                        case 3:
                            if (value > 24)
                                throw new ArgumentException("Invalid hour.");
                            break;
                        case 4:
                        case 5:
                            if (value > 59)
                                throw new ArgumentException($"Invalid {((i == 4) ? "minute" : "second")}.");
                            break;
                    }

                    times[i] = value;
                }

                DateTime dateTime = new DateTime(times[0], times[1], times[2], times[3], times[4], times[5]);
                string imageURL = await ResolveDateToImage(dateTime);

                string fileName = Path.GetFileName(imageURL);

                var emb = new EmbedBuilder()
                    .WithImageUrl($"attachment://{fileName}")
                .Build();
                string message = $"{dateTime.ToString("f")} is...\n";
                await command.RespondWithFileAsync(imageURL, text:message, embed: emb);
            }
            catch (Exception ex)
            {
                string path = LocalAssets.GetTimeImage(IMAGE_NEVER);

                string fileName = Path.GetFileName(path);

                var emb = new EmbedBuilder()
                    .WithImageUrl($"attachment://{fileName}")
                    .Build();

                await command.RespondWithFileAsync(path, embed:emb);
                return;
            }
        }

        private async Task<string> ResolveDateToImage(DateTime time)
        {
            DateTime now = DateTime.Now;

            var difference = time - now;

            TimeSpan span = difference.Duration();

            if(span.Ticks < 0)
            {
                return LocalAssets.GetTimeImage(IMAGE_PAST);
            }

            if (span.Days > 365 * 2)
            {
                return LocalAssets.GetTimeImage(IMAGE_YEARS);
            }
            else if (span.Days > 31 * 2)
            {
                return LocalAssets.GetTimeImage(IMAGE_MONTHS);
            }
            else if (span.Days > 7 * 2)
            {
                return LocalAssets.GetTimeImage(IMAGE_WEEKS);
            }
            else if (span.Days > 1)
            {
                return LocalAssets.GetTimeImage(IMAGE_DAYS);
            }
            else if (span.Hours > 1)
            {
                return LocalAssets.GetTimeImage(IMAGE_HOURS);
            }else if(span.Minutes > 1)
            {
                return LocalAssets.GetTimeImage(IMAGE_MINUTES);
            }else if(span.Seconds > 1)
            {
                return LocalAssets.GetTimeImage(IMAGE_SECONDS);
            }

            return LocalAssets.GetTimeImage(IMAGE_MILLISECONDS);
        }
    }
}

using Discord;
using Discord.WebSocket;
using HyberBot.DailyName;
using HyberBot.DataPersistence.DataFileTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.NeatStuff
{
    public class DailyAutoName : RepeatTask
    {
        private DiscordSocketClient client;

        public DailyAutoName(DiscordSocketClient client, int delay, Action methodToInvoke = null) : base(delay, methodToInvoke)
        {
            this.client = client;
        }

        protected async override Task OnUpdate()
        {
            Logger.Log("DSRN Update call");

            bool modifiedFile = false;
            DateTime now = DateTime.Now;

            DailyNameRecord[] nameRecords = DailyNameController.GetRecords();

            for(int i =0;i<nameRecords.Length;i++)
            {
                if (nameRecords[i] == null)
                {
                    modifiedFile = true;
                    continue;
                }

                DateTime lastNameChange = new DateTime(nameRecords[i].lastNameChangeTime);
                TimeSpan span = now - lastNameChange;

                long tickIntervalRate = nameRecords[i].updateIntervalInTicks;

                if (span.Ticks < tickIntervalRate)
                    continue;
                
                SocketGuild guild = client.GetGuild(nameRecords[i].guildID);
                SocketGuildUser user = guild?.GetUser(nameRecords[i].userID);

                if (guild == null || user == null)
                {
                    nameRecords[i] = null;
                    modifiedFile = true;
                    continue;
                }

                long nowTicks = now.Ticks;
                nameRecords[i].lastNameChangeTime = nowTicks;
                await RenameUser(user);
                modifiedFile = true;
            }

            if(modifiedFile)
            {
                nameRecords = nameRecords.Where(x => x != null).ToArray();
                DailyNameController.UpdateRecords(nameRecords);
            }

        }

        private async Task RenameUser(SocketGuildUser user)
        {
            string newSoulsName = IntruderNamer.GetName();
            await user.ModifyAsync(x=>x.Nickname = newSoulsName);
            Logger.Log($"Renamed User {user.Username} to {newSoulsName} in {user.Guild.Name}");
        }
    }
}

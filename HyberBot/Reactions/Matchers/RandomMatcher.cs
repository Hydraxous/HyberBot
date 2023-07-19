using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.Matchers
{
    public class RandomMatcher : MatchingPattern
    {
        public float PercentChance { get; private set; }

        public RandomMatcher(float percentChance) 
        { 
            PercentChance= percentChance;
        }

        public override bool CheckMatch(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;

            if (userMessage == null)
                return false;

            if (userMessage.Author.IsBot)
                return false;

            return RandomExtensions.PercentRoll(PercentChance);
        }
    }

}

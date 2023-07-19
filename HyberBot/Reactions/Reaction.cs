using Discord;
using Discord.WebSocket;
using HyberBot.Reactions.ReactionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions
{
    [Serializable]
    public class Reaction
    {
        public string Name { get; private set; }
        public bool Enabled { get; private set; } = true;

        public ReactionMethod ReactionMethod { get; private set; }
        public MatchingPattern MatchingPattern { get; private set; }

        public Reaction(string name, ReactionMethod reactionMethod, MatchingPattern matchingPattern)
        {
            this.Name = name;
            this.ReactionMethod = reactionMethod;
            this.MatchingPattern = matchingPattern;
        }

        public void SetActive(bool enabled)
        {
            Enabled = enabled;
        }

        public bool CheckMessage(SocketMessage message)
        {
            if (!Enabled)
                return false;

            return MatchingPattern.CheckMatch(message);
        }

        public virtual async Task ReactAsync(SocketMessage message) 
        {
            await ReactionMethod.React(message);
        }
    }
}

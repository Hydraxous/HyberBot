using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions.Matchers
{
    public class MatchString : MatchingPattern
    {
        public bool IgnoreCase { get; private set; }
        public string StringToMatch { get; private set; }
        public bool Contains { get; private set; }

        public MatchString(string stringToMatch, bool ignoreCase = true, bool contains = true) 
        {
            this.StringToMatch = stringToMatch;
            this.IgnoreCase = ignoreCase;
            this.Contains = contains;
        }

        public override bool CheckMatch(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;

            if (userMessage == null)
                return false;

            if (userMessage.Author.IsBot)
                return false;



            string word = (IgnoreCase) ? StringToMatch.ToLower() : StringToMatch;
            string msg = (IgnoreCase) ? message.Content.ToLower() : message.Content;

            if(!Contains)
                return msg == word;
           
            return msg.Contains(word);
        }
    }
}

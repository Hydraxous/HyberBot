using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.Reactions
{
    public class Reactor
    {
        private List<Reaction> reactions;

        private DiscordSocketClient _client;
        public Reactor(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += _client_Ready;
            reactions = new List<Reaction>();
            Logger.Log("Reactor created!");
        }

        private async Task _client_Ready()
        {
            _client.MessageReceived += InterpretMessage;
            await Task.CompletedTask;
        }

        public void AddReaction(Reaction reaction)
        {
            if(reactions.Contains(reaction))
            {
                Logger.LogError("You cannot add two reactions of the same name.");
                return;
            }

            reactions.Add(reaction);
        }
        
        public bool ContainsReactionByName(string name)
        {
            return GetReaction(name) != null;
        }

        public void SetReactions(List<Reaction> newReactions)
        {
            reactions = newReactions;
        }

        public List<Reaction> GetReactions()
        {
            return reactions;
        }

        public Reaction GetReaction(string name)
        {
            foreach (Reaction reaction in reactions)
            {
                if (reaction.Name == name)
                    return reaction;
            }

            return null;
        }

        public void RemoveReaction(string name)
        {
            Reaction foundReaction = GetReaction(name);

            if (foundReaction != null)
                reactions.Remove(foundReaction);
        }

        public void RemoveReaction(Reaction reaction)
        {
            if (reactions.Contains(reaction))
                reactions.Remove(reaction);
        }

        private async Task InterpretMessage(SocketMessage message)
        {
            Logger.Log($"Recieved message {message.Content} from {message.Author.Username}");

            foreach(Reaction reaction in reactions)
            {
                if (!reaction.CheckMessage(message))
                    continue;

                Logger.Log($"Reacting to message with {reaction.Name}!");
                await reaction.ReactAsync(message);
            }
        }

    }
}

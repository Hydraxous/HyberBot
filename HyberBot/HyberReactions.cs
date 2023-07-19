using Discord;
using HyberBot.Reactions;
using HyberBot.Reactions.Matchers;
using HyberBot.Reactions.ReactionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public class HyberReactions
    {
        Hyber hyber;

        public Reactor Reactor { get; private set; }

        public HyberReactions(Hyber hyber) 
        {
            this.hyber = hyber;
            Reactor = new Reactor(hyber.Client);

            AddStringEmojiReaction("horse_hungry", "hungry", "\U0001F434");
            AddStringEmojiReaction("cat_meow", "meow", "\U0001F408");
            AddStringImageReaction("inacoupleweeks", "weeks", "https://cdn.discordapp.com/attachments/1067476876618125386/1117878294637977700/SmartSelect_20230527-011006.png");
            AddRandomPercentReaction("nerdReactImageRandom", 0.01f, "https://media.tenor.com/SRX8X6DNF6QAAAAd/nerd-nerd-emoji.gif");
        }

        private void AddStringEmojiReaction(string name, string word, string emojiUnicode)
        {
            Emoji eomji = new Emoji(emojiUnicode);
            if (eomji == null)
            {
                Logger.LogError($"{emojiUnicode} is not a valid emoji.");
                return;
            }
            EmoteReactionMethod emojiReactionMethod = new EmoteReactionMethod(eomji);
            MatchString matchString = new MatchString(word);
            Reaction reaction = new Reaction(name, emojiReactionMethod, matchString);
            Reactor.AddReaction(reaction);

            Logger.Log($"{name} reaction added.");
        }

        private void AddStringImageReaction(string name, string word, string imageLink)
        {
            ImageReaction imageReaction = new ImageReaction(imageLink);
            MatchString matchString = new MatchString(word);
            Reaction reaction = new Reaction(name, imageReaction, matchString);
            Reactor.AddReaction(reaction);

            Logger.Log($"{name} image reaction added.");
        }

        private void AddRandomPercentReaction(string name, float percent, string imageLink)
        {
            ImageReaction imageReaction = new ImageReaction(imageLink);
            RandomMatcher randomMatcher = new RandomMatcher(percent);
            Reaction reaction = new Reaction(name, imageReaction, randomMatcher);

            Reactor.AddReaction(reaction);
            Logger.Log($"Random Reaction added {name}");
        }
    }
}

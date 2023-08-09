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
            AddRandomPercentImageReaction("nerdReactImageRandom", 0.0033f, "https://media.tenor.com/SRX8X6DNF6QAAAAd/nerd-nerd-emoji.gif");
            AddRandomPercentImageReaction("speechBubbleReact0", 0.002f, "https://i.imgflip.com/76opl0.jpg");
            AddRandomPercentImageReaction("speechBubbleReact1", 0.002f, "https://pbs.twimg.com/media/FSDB4HmaAAEynu4.jpg");
            AddStringImageReaction("ilikechad", "i like", "https://media.tenor.com/5uk8htBy9ncAAAAd/squidward-chad.gif");
            AddRandomPercentEmojiReaction("fishReactRandom", 0.002f, "\U0001F41F");
            AddRandomPercentEmojiReaction("nerdReactEmojiRandom", 0.002f, "\U0001F913");
            AddRandomPercentEmojiReaction("scorpionEmojiReactRandom", 0.002f, "\U0001F982");
            AddRandomPercentEmojiReaction("questionEmojiReactRandom", 0.002f, "\U00012753");
            AddRandomPercentEmojiReaction("pointUpEmojiReactRandom", 0.002f, "\U0001261D");
            AddRandomPercentEmojiReaction("checkMarkEmojiReactRandom", 0.002f, "\U00012713");
            AddRandomPercentEmojiReaction("laughEmojiReactRandom", 0.002f, "\U0001F602");
            AddRandomPercentEmojiReaction("expressionlessEmojiReactRandom", 0.002f, "\U0001F611");
            AddRandomPercentEmojiReaction("grimacingEmojiReactRandom", 0.002f, "\U0001F62C");
            AddRandomPercentEmojiReaction("liarEmojiReactRandom", 0.002f, "\U0001F925");
            AddRandomPercentEmojiReaction("skullEmojiReactRandom", 0.002f, "\U0001F480");
            AddRandomPercentEmojiReaction("pensiveEmojiReactRandom", 0.002f, "\U0001F614");
            AddRandomPercentEmojiReaction("pleadingEmojiReactRandom", 0.002f, "\U0001F97A");
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

        private void AddRandomPercentImageReaction(string name, float percent, string imageLink)
        {
            ImageReaction imageReaction = new ImageReaction(imageLink);
            RandomMatcher randomMatcher = new RandomMatcher(percent);
            Reaction reaction = new Reaction(name, imageReaction, randomMatcher);

            Reactor.AddReaction(reaction);
            Logger.Log($"Random Reaction added {name}");
        }

        private void AddRandomPercentEmojiReaction(string name, float percent, string emojiUnicode)
        {
            Emoji eomji = new Emoji(emojiUnicode);
            if (eomji == null)
            {
                Logger.LogError($"{emojiUnicode} is not a valid emoji.");
                return;
            }
            EmoteReactionMethod emojiReactionMethod = new EmoteReactionMethod(eomji);
            RandomMatcher randomMatcher = new RandomMatcher(percent);
            Reaction reaction = new Reaction(name, emojiReactionMethod, randomMatcher);
            
            Reactor.AddReaction(reaction);
        }
    }
}

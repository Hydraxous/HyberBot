using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DataPersistence.DataFileTypes
{
    [Serializable]
    public class GuildData : Validatable
    {

        public List<DailyNameRecord> dailyRenameUsers;

        public List<Reactions.Reaction> reactions;

        public string dataMessage;

        public override bool Validate()
        {
            if (dailyRenameUsers == null)
                return false;

            if (reactions == null)
                return false;

            return true;
        }

        public GuildData()
        {
            dataMessage = "Default Message";
            dailyRenameUsers = new List<DailyNameRecord>();
            reactions = new List<Reactions.Reaction>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DataPersistence.DataFileTypes
{
    [Serializable]
    public class DailyNameRoster : Validatable
    {
        public List<DailyNameRecord> records;

        public DailyNameRoster()
        {
            records = new List<DailyNameRecord>();
        }

        public DailyNameRoster(List<DailyNameRecord> records)
        {
            this.records = records;
        }

        public override bool Validate()
        {
            return records != null;
        }
    }

    [Serializable]
    public class DailyNameRecord
    {
        public ulong guildID;
        public ulong userID;
        public long lastNameChangeTime;
        public long updateIntervalInTicks;
    }
}

using HyberBot.DataPersistence;
using HyberBot.DataPersistence.DataFileTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DailyName
{
    public static class DailyNameController
    {
        private static string filePath => Path.Combine(DataManager.DataFolder, "global", "dailyNameRoster.json");

        private static List<DailyNameRecord> _roster;

        private static List<DailyNameRecord> roster
        {
            get
            {
                if (_roster == null)
                {
                    LoadRecords();
                }

                return _roster;
            }
        }

        private static void SaveRecords()
        {
            if (_roster == null)
                return;

            DataManager.SaveData<List<DailyNameRecord>>(filePath, _roster);
        }

        private static void LoadRecords()
        {
            if(DataManager.TryLoadData<List<DailyNameRecord>>(filePath, out List<DailyNameRecord> loadedList))
            {
                _roster = loadedList;
            }else
            {
                _roster = new List<DailyNameRecord>();
                UpdateRecords(_roster.ToArray());
            }
        }

        public static void UpdateRecords(DailyNameRecord[] records)
        {
            _roster = records.ToList();
            Logger.Log("Daily Name Roster updated.");
            SaveRecords();
        }

        public static DailyNameRecord[] GetRecords()
        {
            return roster.ToArray();
        }

        public static void AddRecord(ulong guildID, ulong userID, long timeInterval)
        {

            DailyNameRecord[] existingRecords = GetRecords();

            for (int i = 0; i < existingRecords.Length; i++)
            {
                if (existingRecords[i].guildID != guildID)
                    continue;

                if (existingRecords[i].userID != userID)
                    continue;

                existingRecords[i].updateIntervalInTicks = timeInterval;
                existingRecords[i].lastNameChangeTime = 0;
                UpdateRecords(existingRecords);
                return;
            }

            DailyNameRecord newRecord = new DailyNameRecord();
            newRecord.guildID = guildID;
            newRecord.userID = userID;
            newRecord.updateIntervalInTicks = timeInterval;

            List<DailyNameRecord> recordList = existingRecords.ToList();
            recordList.Add(newRecord);
            UpdateRecords(recordList.ToArray());
        }

        public static void RemoveRecord(ulong guildID, ulong userID)
        {
            DailyNameRecord[] existingRecords = GetRecords();

            for (int i = 0; i < existingRecords.Length; i++)
            {
                if (existingRecords[i].guildID != guildID)
                    continue;

                if (existingRecords[i].userID != userID)
                    continue;

                existingRecords[i] = null;
            }

            existingRecords = existingRecords.Where(x => x != null).ToArray();
            UpdateRecords(existingRecords);
        }
    }
}

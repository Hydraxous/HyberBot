using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HyberBot.DataPersistence
{
    public static class GuildDataManager
    {

        private static Dictionary<ulong, GuildDataDirectory> dataDirectories = new Dictionary<ulong, GuildDataDirectory>();

        private static string dataFolder => Path.Combine(Directory.GetCurrentDirectory(), "data");


        public static bool SaveGuildData<T>(ulong guildID, string fileName, T guildData) where T : Validatable
        {
            string folder = Path.Combine(dataFolder, guildID.ToString());
            string filePath = Path.Combine(folder, fileName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            try
            {
                if (!guildData.Validate())
                {
                    Logger.LogError($"Could not save file {fileName}");
                    return false;
                }

                string json = JsonConvert.SerializeObject(guildData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Logger.Log($"Saved data file {fileName} successfully.");
                return true;

            }catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return false;
        }

        private static bool TryLoadDataFile<T>(ulong guildID, string fileName, out T data) where T : Validatable
        {
            data = null;

            string filePath = Path.Combine(dataFolder, guildID.ToString(), fileName);

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string json;

                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    json = streamReader.ReadToEnd();
                }


                data = JsonConvert.DeserializeObject<T>(json);

                if (!data.Validate())
                    return false;

                return true;

            }catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return false;
        }


        public static T GetGuildData<T>(ulong guildID, string fileName) where T : Validatable
        {
            if (!dataDirectories.ContainsKey(guildID))
            {
                dataDirectories.Add(guildID, new GuildDataDirectory(guildID));
            }

            GuildDataDirectory guildData = dataDirectories[guildID];

            Validatable data = null;

            //Try to load it from disk
            if (guildData.files.ContainsKey(fileName))
            {
                return (T)guildData.files[fileName];
            }

            if (TryLoadDataFile<T>(guildID, fileName, out T loadedData))
            {
                data = loadedData;
            }
            else
            {
                data = Activator.CreateInstance<T>();
            }

            guildData.files.Add(fileName, data);

            return (T) data;
        }

        public static void ClearCache()
        {
            dataDirectories.Clear();
        }

        public static bool ForceReloadFile<T>(ulong guildID, string fileName) where T : Validatable
        {
            if (!dataDirectories.ContainsKey(guildID))
            {
                dataDirectories.Add(guildID, new GuildDataDirectory(guildID));
            }

            GuildDataDirectory guildData = dataDirectories[guildID];

            Validatable data = null;

            if (!TryLoadDataFile<T>(guildID, fileName, out T loadedData))
            {
                return false;
            }

            data = loadedData;

            if(!guildData.files.ContainsKey(fileName))
                guildData.files.Add(fileName, data);
            else
                guildData.files[fileName] = data;
            
            return true;
        }
    }

    public class GuildDataDirectory
    {
        public ulong GuildID { get; private set; }

        public GuildDataDirectory(ulong guildID)
        {
            GuildID = guildID;
        }

        public Dictionary<string, Validatable> files = new Dictionary<string, Validatable>();
    }
}

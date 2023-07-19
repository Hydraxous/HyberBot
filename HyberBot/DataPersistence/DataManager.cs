using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DataPersistence
{
    public static class DataManager
    {
        public static string DataFolder => Path.Combine(Directory.GetCurrentDirectory(), "data");
        public static string GlobalDataFolder => Path.Combine(DataFolder, "global");

        private static Dictionary<string,Validatable> cachedDataFiles = new Dictionary<string,Validatable>();

        public static bool SaveValidatableData<T>(string filePath, T data) where T : Validatable
        {
            string folder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            try
            {
                if (!data.Validate())
                {
                    Logger.LogError($"Could not save file {fileName}");
                    return false;
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Logger.Log($"Saved data file {fileName} successfully.");
                return true;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return false;
        }

        private static bool TryLoadValidatableData<T>(string filePath, out T data) where T : Validatable
        {
            data = null;

            string folder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

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

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return false;
        }

        public static bool TryLoadData<T>(string filePath, out T data)
        {
            data = default(T);

            string folder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

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

                if (data != null)
                    return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return false;
        }

        public static void SaveData<T>(string filePath, T data)
        {

            string folder = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Logger.Log($"Saved data file {fileName} successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        public static T GetValidatableData<T>(string filePath) where T : Validatable
        {
            if (cachedDataFiles.ContainsKey(filePath))
            {
                return (T) cachedDataFiles[filePath];
            }

            Validatable data = null;

            if (TryLoadValidatableData<T>(filePath, out T loadedData))
            {
                data = loadedData;
            }
            else
            {
                data = Activator.CreateInstance<T>();
            }

            cachedDataFiles.Add(filePath, data);
            return (T)data;
        }
    }
}

using HyberBot;
using HyberBot.DataPersistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IntruderNamer
{
    private static IntruderNames names;
    public static IntruderNames Names
    { 
        get
        {
            if(names == null)
            {
                LoadNames();
            }
            return names;
        }
    }

    private const string NAMES_FILENAME = "soulsnames.txt";
    private static string filePath => Path.Combine(DataManager.GlobalDataFolder, NAMES_FILENAME);


    public static bool NameListContains(string name)
    {
        foreach(var storedName in Names.names)
        {
            if (storedName == name)
                return true;
        }

        return false;
    }

    public static string[] GetAllNames()
    {
        return Names.names;
    }

    public static void AddNewName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        List<string> names = Names.names.ToList();
        names.Add(name);
        Names.names = names.ToArray();
        SaveNames();
    }

    private static void LoadNames()
    {
        if(DataManager.TryLoadData<IntruderNames>(filePath, out IntruderNames data))
        {
            names = data;
        }else
        {
            names = new IntruderNames();
            SaveNames();
        }
    }

    private static void SaveNames()
    {
        if (names == null)
            return;

        DataManager.SaveData<IntruderNames>(filePath, names);
        Logger.Log("Saved Intruder Names File");
    }

    public static string GetName(bool forceMush = false)
    {
        if(Names.jumbledNames)
        {
            if(RandomExtensions.RollXAnd(2) || forceMush)
            {
                int wordCount = (RandomExtensions.BinaryRandom()) ? 1 : 2; 
                string jumbleName = GenerateMushedName(wordCount, RandomExtensions.BinaryRandom(), RandomExtensions.RollXAnd(3));
                return jumbleName;
            }
        }

        return Names.names.RandomEntry();
    }

    private static readonly string[] prefixes = { 
        "Lord", 
        "Lady", 
        "Evil", 
        "High Lord", 
        "CrustMaxxer", 
        "Primal", 
        "Badass", 
        "Darth", 
        "MEGA", 
        "GIGA", 
        "Chadlike",
        "Rogue", 
        "Blue", 
        "Red", 
        "Black", 
        "White", 
        "Round", 
        "Big", 
        "Little", 
        "Small", 
        "Large"
    };

    public static string GenerateMushedName(int words, bool tryAddSuffix = false, bool tryAddPrefix = false)
    {
        IntruderNames names = new IntruderNames();

        List<string> randomWords = new List<string>();
        string suffix = "";

        List<string> possibleNames = names.names.ToList();
        possibleNames.Shuffle();

        List<string> newName = new List<string>();

        foreach(string name in possibleNames)
        {
            string[] subwords = name.Split(' ');
            if (subwords == null)
                continue;

            if (subwords.Length == 0)
                continue;

            if (subwords.Length == 1 && RandomExtensions.BinaryRandom())
                continue;


            for (int i = 0; i < subwords.Length; i++)
            {
                if (CheckBlacklistChars(subwords[i]))
                    continue;

                if ((subwords[i].EndsWith("er") || RandomExtensions.RollXAnd(2)) && tryAddSuffix)
                {
                    suffix = subwords[i];

                    if(RandomExtensions.RollXAnd(10))
                    {
                        suffix = "- THE GOD BEING";
                    }else if(RandomExtensions.RollXAnd(4))
                    {
                        suffix = "- the man himself...";
                    }else if (RandomExtensions.RollXAnd(2))
                    {
                        suffix += RandomExtensions.Range(0, 1000).ToString("000");
                    }
                }
                else
                {
                    if (!randomWords.Contains(subwords[i]) || RandomExtensions.BinaryRandom())
                    {
                        randomWords.Add(subwords[i]);
                    }
                }
            }
        }


        if (tryAddPrefix && RandomExtensions.RollXAnd(4))
            newName.Add(prefixes.RandomEntry());

        for (int i = 0; i < words; i++)
        {
            newName.Add(randomWords.RandomEntry());
        }

        if(tryAddSuffix)
            newName.Add(suffix);

        return string.Join(' ', newName);
    }

    private static readonly string mushNameCharBlacklist = "-1234567890$?)(";

    private static bool CheckBlacklistChars(string str)
    {
        for(int i=0;i<mushNameCharBlacklist.Length;i++)
        {
            if (str.Contains(mushNameCharBlacklist[i]))
                return true;
        }
        return false;
    }

    public static string GetFilteredName()
    {
        return GetName().Replace("knight", "k***ht");
    }
}

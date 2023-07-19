using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public static class LocalAssets
    {
        private static string AssetFolder => Path.Combine(Directory.GetCurrentDirectory(), "assets");

        
        public static string GetTimeImage(string fileName)
        {
            string path = Path.Combine(AssetFolder, "images", "times", fileName);

            if (!File.Exists(path))
                return string.Empty;

            return path;
        }



    }
}

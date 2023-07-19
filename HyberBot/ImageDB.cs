using HyberBot.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public static class ImageDB
    {
        private static Dictionary<string, ImageLink> images = new Dictionary<string, ImageLink>()
        {

        };

        public static string GetImageURLByName(string name)
        {
            if (!images.ContainsKey(name))
                return string.Empty;

            return images[name].URL;
        }
    }
}

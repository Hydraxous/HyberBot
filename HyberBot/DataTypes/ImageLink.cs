using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot.DataTypes
{
    public struct ImageLink
    {
        public string URL { get; private set; }

        public string Name { get; private set; }

        public ImageLink(string name, string url)
        {
            Name = name;
            URL = url;
        }
    }
}

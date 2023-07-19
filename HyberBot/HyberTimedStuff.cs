using HyberBot.NeatStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public class HyberTimedStuff
    {
        Hyber hyber;

        private DailyAutoName autoName;

        public HyberTimedStuff(Hyber hyber) 
        {
            this.hyber = hyber;
            autoName = new DailyAutoName(hyber.Client, 60000);
        }

        public void Start()
        {
            autoName.Start();
        }

    }
}

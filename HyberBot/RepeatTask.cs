using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    public class RepeatTask
    {
        public bool Running { get; private set; }
        private int delay;

        public RepeatTask(int delay, Action invokeMethod = null)
        {
            this.delay = delay;
            this.invokeMethod = invokeMethod;
        }

        public void Start()
        {
            if (Running)
                return;
            Logger.Log("Started background task!!!");
            Task.Run(Loop);
        }

        private Action onStopCallback;
        private Action invokeMethod;

        public void Stop(Action onStoppedCallback)
        {
            Running = false;
            onStopCallback= onStoppedCallback;
        }

        private async Task Loop()
        {
            Running = true;

            while (Running)
            {
                await OnUpdate();
                await Task.Delay(delay);
            }

            onStopCallback?.Invoke();
        }

        protected virtual async Task OnUpdate()
        {
            invokeMethod?.Invoke();
        }

    }
}

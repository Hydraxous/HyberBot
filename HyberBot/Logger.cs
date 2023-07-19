using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyberBot
{
    internal static class Logger
    {
        private static List<string> messages = new List<string>();

        private static string prefix = "HYBER";

        public static void DLog(LogMessage message)
        {
            LogWithPrefix($"{message.Message}\n{message.Exception}" , message.Source);
        }

        public static Task DLogAsync(LogMessage message)
        {
            DLog(message);
            return Task.CompletedTask;
        }

        public static void LogWithPrefix(object obj, string prefix)
        {
            if (obj == null)
                return;

            string msg = BuildMessage(obj.ToString(), prefix);
            messages.Add(msg);
            Console.WriteLine(msg);
        }

        public static void Log(object obj) 
        {
            if (obj == null)
                return;

            string msg = BuildMessage(obj.ToString(), prefix);
            messages.Add(msg);
            Console.WriteLine(msg);
        }

        public static void LogError(object obj)
        {
            if (obj == null)
                return;

            string msg = BuildMessage(obj.ToString(), "ERROR");
            messages.Add(msg);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogWarning(object obj)
        {
            if (obj == null)
                return;

            string msg = BuildMessage(obj.ToString(), "WARNING");
            messages.Add(msg);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static string BuildMessage(string msg, string prefix)
        {
            return $"[{DateTime.Now.ToString("T")}][{prefix}]: {msg}";
        }

    }
}

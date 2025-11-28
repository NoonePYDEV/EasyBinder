using System;

namespace  EasyBinder
{
    class Logger
    {
        public static void Info(string Message)
        {
            Console.WriteLine("\u001b[34m[\u001b[0m  INFO   \u001b[34m]\u001b[0m " + Message.Trim());
        }

        public static void Error(string Message, bool Exit = false)
        {
            Console.WriteLine("\u001b[31m[\u001b[0m  ERROR  \u001b[31m]\u001b[0m " + Message.Trim());

            if (Exit)
                Environment.Exit(0);
        }

        public static void Warning(string Message)
        {
            Console.WriteLine("\u001b[33m[\u001b[0m WARNING \u001b[33m]\u001b[0m " + Message.Trim());
        }

        public static void Success(string Message)
        {
            Console.WriteLine("\u001b[32m[\u001b[0m SUCCESS \u001b[32m]\u001b[0m " + Message.Trim());
        }
    }
}
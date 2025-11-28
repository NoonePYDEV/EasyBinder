using System;

namespace EasyBinder
{
    class ArgsParser
    {
        public static string GetArgumentValue(string Arg)
        {
            try
            {
                return Arg.Split("=")[1];
            }
            catch
            {
                Logger.Error("Invalid argument : " + Arg, true);
                return "";
            }
        } 
    }
}
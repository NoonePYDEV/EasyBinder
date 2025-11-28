using System;
using System.Security.Cryptography.X509Certificates;

namespace EasyBinder
{
    class EasyBinderConfig
    {
        public string DistFileName = "Done";
        public string WorkDir = ".\\EzBinder.Build";
        public string OutputDir = ".\\EzBinder.Output";

        public List<string> Executables = new List<string> { };
        public bool ExcludeRuntime = false;
        public bool NoConsole = false;

        public EasyBinderConfig()
        {

        }
    }
}
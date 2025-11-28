using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyBinder
{
    class TableDrawer
    {
        public static void DrawTableFromConfig(EasyBinderConfig Config)
        {
            Dictionary<string, string> Values = new Dictionary<string, string>
            {
                { "Dist File Name", Config.DistFileName },
                { "Output Directory", Config.OutputDir },
                { "Work Directory", Config.WorkDir },
                { "Hide Console", Config.NoConsole.ToString() },
                { "Exclude .NET Runtime", Config.ExcludeRuntime.ToString() }
            };

            int count = 1;

            foreach (string ExePath in Config.Executables)
            {
                string Key = $"Executable {count}";
                string Value = Path.GetFileName(ExePath);

                Values.Add(Key, Value);
                count++;
            }

            int MaxKeyWidth = Values.Keys.Max(k => k.Length);
            int MaxValueWidth = Values.Values.Max(v => v.Length);

            MaxKeyWidth += 2;
            MaxValueWidth += 2;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\u001b[34m+" + new string('-', MaxKeyWidth + MaxValueWidth + 5) + "+\u001b[0m");

            foreach (var Pair in Values)
            {
                string FormattedKey = Pair.Key.PadRight(MaxKeyWidth);
                string FormattedValue = Pair.Value.PadRight(MaxValueWidth);

                sb.AppendLine($"\u001b[34m|\u001b[0m {FormattedKey} \u001b[34m|\u001b[0m {FormattedValue} \u001b[34m|\u001b[0m");
            }

            sb.AppendLine("\u001b[34m+" + new string('-', MaxKeyWidth + MaxValueWidth + 5) + "+\u001b[0m");

            Console.WriteLine(sb.ToString());
        }
    }
}
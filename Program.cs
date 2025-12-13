using System;
using System.Diagnostics;

namespace EasyBinder
{
    class Program
    {
        static void Main(string[] Args)
        {
            if (Args.Length == 0)
            {
                Console.WriteLine("Usage : ezbinder [ Options ] <DistFileName>");
                return;
            }
            else if (Args.Contains("-h") || Args.Contains("--help"))
            {
                Console.WriteLine(Help.HelpInfos);
                return;
            }

            EasyBinderConfig Config = new EasyBinderConfig();

            foreach (string Arg in Args)
            {
                if (!Arg.StartsWith("--") && !Arg.StartsWith("-"))
                {
                    foreach (char _char in Arg.ToCharArray())
                    {
                        if (Path.GetInvalidFileNameChars().Contains(_char))
                            Logger.Error("The dist file name contains invalid characters.", true);
                        else
                            Config.DistFileName = Arg;
                    }
                }
            }

            if (string.IsNullOrEmpty(Config.DistFileName))
                Logger.Error("Please provide a dist file name.", true);

            foreach (string Arg in Args)
            {
                if (Arg == "--exclude-runtime")
                {
                    Config.ExcludeRuntime = true;
                }
                else if (Arg == "--noconsole")
                {
                    Config.NoConsole = true;
                }
                else if (Arg.StartsWith("--workpath"))
                {
                    Config.WorkDir = ArgsParser.GetArgumentValue(Arg);
                }
                else if (Arg.StartsWith("--outputdir"))
                {
                    Config.OutputDir = ArgsParser.GetArgumentValue(Arg);
                }
                else if (Arg.StartsWith("--bind-file"))
                {
                    string ExePath = ArgsParser.GetArgumentValue(Arg);

                    if (!File.Exists(ExePath))
                        Logger.Error("File not found : " + ExePath, true);

                    Config.Executables.Add(ExePath);
                }
                else
                {
                    if (Arg != Config.DistFileName)
                        Logger.Warning("Skipping an unknown argument : " + Arg);
                }
            }

            if (Config.Executables.Count <= 1)
                Logger.Error("Please provide at least 2 executables to bind.", true);

            TableDrawer.DrawTableFromConfig(Config);

            Logger.Info($"Processing {Config.Executables.Count} executables to bind");

            try
            {
                Directory.CreateDirectory(Config.WorkDir);
                Directory.CreateDirectory(Config.OutputDir);
            }
            catch (Exception ex)
            {
                Logger.Error("Couldn't setup the build environment : " + ex.Message, true);
            }

            string CSProj = Payloads.BaseCSProj;
            string ProgramMain = Payloads.BaseMainPayload;

            if (Config.NoConsole)
                CSProj = CSProj.Replace("{__EXE_MODE__}", "WinExe");
            else
                CSProj = CSProj.Replace("{__EXE_MODE__}", "Exe");

            Process DotnetProject = new Process();

            DotnetProject.StartInfo.FileName = "dotnet.exe";
            DotnetProject.StartInfo.Arguments = $"new console -o {Path.Combine(Config.WorkDir, "__SIGMA_CS__")}";
            DotnetProject.StartInfo.CreateNoWindow = true;

            DotnetProject.Start();
            DotnetProject.WaitForExit();

            if (DotnetProject.ExitCode != 0)
                Logger.Error($"Couldn't create the dotnet project : return code {DotnetProject.ExitCode} from dotnet", true);

            string CurrentFileName = "lIlIlIlIlIlIlIlIlIlIlIlIlIlIlIlIlIlIl";

            List<string> FileNames = new List<string> { };
            List<string> EmbedXMLs = new List<string> { };

            foreach (string ExePath in Config.Executables)
            {
                Logger.Info("Copying " + ExePath);

                string ProjectDir = Path.Combine(Config.WorkDir, "__SIGMA_CS__");
                string DestFile = Path.Combine(ProjectDir, CurrentFileName + ".exe");

                File.Copy(ExePath, DestFile, overwrite: true);
                FileNames.Add(CurrentFileName);

                string RelativePath = Path.GetFileName(DestFile);
                EmbedXMLs.Add(Payloads.BaseEmbeddedRsrcXML.Replace("{__FILE_NAME__}", RelativePath));

                if (CurrentFileName.EndsWith("i"))
                    CurrentFileName += "l";
                else
                    CurrentFileName += "I";
            }

            Logger.Info("Creating the base project");

            CSProj = CSProj.Replace("{__EMBED_XMLs__}", string.Join("\n", EmbedXMLs));

            List<string> EmbeddedExtractionPayloads = new List<string> { };

            foreach (string FileName in FileNames)
            {
                EmbeddedExtractionPayloads.Add(Payloads.BaseEmbbededRscrcLoader.Replace("{__FILE_NAME__}", FileName).Replace("{__CNW__}", Config.NoConsole.ToString().ToLower()));
            }

            ProgramMain = ProgramMain.Replace("{__EMBEDDED_PAYLOADS_EXTRACTION__}", string.Join("\n\n", EmbeddedExtractionPayloads));

            File.WriteAllText(Path.Combine(Config.WorkDir, "__SIGMA_CS__", "__SIGMA_CS__.csproj"), CSProj);
            File.WriteAllText(Path.Combine(Config.WorkDir, "__SIGMA_CS__", "Program.cs"), ProgramMain);

            Logger.Info("Compiling");

            Process DotnetCompiler = new Process();

            DotnetCompiler.StartInfo.FileName = "dotnet.exe";
            DotnetCompiler.StartInfo.Arguments = $"publish {Path.Combine(Config.WorkDir, "__SIGMA_CS__", "__SIGMA_CS__.csproj")} -o \"{Config.OutputDir}\" /p:AssemblyName=\"{Config.DistFileName.Replace(".exe", "")}\" /p:PublishSingleFile=true /p:SelfContained={(Config.ExcludeRuntime == false).ToString().ToLower()}";
            DotnetCompiler.StartInfo.CreateNoWindow = true;

            DotnetCompiler.Start();
            DotnetCompiler.WaitForExit();

            if (DotnetCompiler.ExitCode != 0)
                Logger.Error($"An error occured during compilation : return code {DotnetCompiler.ExitCode} from dotnet", true);

            try
            {
                File.Delete(Path.Combine(Config.WorkDir, "__SIGMA_CS__", "Program.cs"));
                File.Delete(Path.Combine(Config.WorkDir, "__SIGMA_CS__", "__SIGMA_CS__.csproj"));
            }
            catch {}

            Logger.Success($"Compilation finished. Your binded payload is available in {Config.OutputDir}");
        }
    }

}

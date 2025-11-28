using System;

namespace EasyBinder
{
    class Help
    {
        public static string HelpInfos = @"
    Usage : ezbinder [ Options ] <DistFileName>

    Options:
    --bind-file=<ExeFilePath>
    --encrypt                    Encrypt the binded executables (makes the process a little slower).
    --pack                       Enable compression or optimization for the executables.
    --noconsole                  Hide the console window when running the executable (useful for GUI apps).
    --outputdir=<Directory>      Specify the output directory for the executable.
    --workpath=<Directory>       Set the temporary working directory for the compilation process.
    --exclude-runtime            Exclude .NET Runtime, it makes the build smaller and faster but some computer will not be able to run it if .NET is not installed

    Examples:
    ezbinder --bind-file=FirstPayload.exe --bind-file=SecondPayload.exe --noconsole MyFinalPayloadName.exe
    ";
    }
}
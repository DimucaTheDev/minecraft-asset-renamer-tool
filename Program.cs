using System.Net;
using System.Text.Json;

namespace MinecraftAssetRenamer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string outputPath = (args.Length > 0 ? args[0].Replace("\"", "") : "output") + "/";

            Console.Write("Enter version json file path: ");

            string jsonPath = Console.ReadLine()!.Replace("\"", "")!;
            string versionJson = File.ReadAllText(jsonPath);
            string directoryName = Path.GetDirectoryName(jsonPath)!;
            string root = (new DirectoryInfo(directoryName).Parent!.Parent!.FullName);// ../..    gets minecraft root folder
            string assetsPath = Path.Combine(root + "/assets/objects/");
            var jsonElement = JsonDocument.Parse(versionJson).RootElement.GetProperty("assetIndex");
            string existedAssetsJson = $"{root}/assets/indexes/{jsonElement.GetProperty("id").GetString()}.json";
            string assetsJson = File.Exists(existedAssetsJson) ? File.ReadAllText(existedAssetsJson) : new WebClient().DownloadString(jsonElement.GetProperty("url").GetString()!);

            JsonDocument.Parse(assetsJson).RootElement.GetProperty("objects").EnumerateObject().ToList().ForEach(a =>
            {
                string hash = a.Value.GetProperty("hash").GetString()!;
                var destFileName = outputPath + a.Name;
                var sourceFileName = assetsPath + $"/{hash[..2]}/{hash}";

                Console.WriteLine($"{hash[..2]}/{hash} -> {a.Name}");

                if (File.Exists(destFileName)) return;
                if (!File.Exists(sourceFileName))
                {
                    Console.WriteLine($"{sourceFileName} NOT EXISTS");
                    return;
                }
                Directory.CreateDirectory(outputPath + Path.GetDirectoryName(a.Name));
                File.Copy(sourceFileName, destFileName);
            });
        }
    }
}

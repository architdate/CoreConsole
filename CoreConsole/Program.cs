using System;
using System.IO;
using System.Linq;
using PKHeX.Core;

namespace CoreConsole
{
    internal static class Program
    {
        private static readonly string appPath = Environment.CurrentDirectory;
        private const string RequestLegalityCheck = "-l";
        private const string RequestLegalization = "-alm";

        private static void Main(string[] args)
        {
            var pk = CreatePKMfromArgs(args);

            if (args.Contains(RequestLegalityCheck))
            {
                var lc = new LegalityAnalysis(pk);
                var verbose = args.Contains("--verbose");
                Console.WriteLine(lc.Report(verbose));
            }

            if (!args.Contains(RequestLegalization))
                return;

            if (!args.Contains("--version"))
            {
                Console.WriteLine("Specify version with the [--version] tag");
                return;
            }

            var ver = GetArgVal(args, "--version");
            pk = ConsoleLegalizer.GetLegalPKM(pk, ver);
            var outPath = GetOutputPath(args, pk);
            File.WriteAllBytes(outPath, pk.DecryptedBoxData);
        }

        private static string GetOutputPath(string[] args, PKM p)
        {
            if (args.Contains("-o"))
                return GetArgVal(args, "-o");
            return Path.Combine(appPath, "output", Util.CleanFileName(p.FileName));
        }

        private static PKM CreatePKMfromArgs(string[] args)
        {
            // check -i for input and get file path in the next arg
            if (args.Contains("-i"))
            {
                var path = GetArgVal(args, "-i");
                var data = File.ReadAllBytes(path);
                return PKMConverter.GetPKMfromBytes(data);
            }

            var gameStr = GetArgVal(args, "--version");
            var parsed = Enum.TryParse<GameVersion>(gameStr, true, out var game);
            if (!parsed)
            {
                Console.WriteLine("Invalid version specified: " + gameStr);
                game = GameVersion.Any;
            }

            var showdownSet = GetArgVal(args, "--set");
            var template = PKMConverter.GetBlank(game.GetGeneration(), game);
            template.ApplySetDetails(new ShowdownSet(showdownSet.Split(new [] { "\\n" }, StringSplitOptions.None)));
            return template;
        }

        private static string GetArgVal(string[] args, string modifier) => args[Array.IndexOf(args, modifier) + 1];
    }
}

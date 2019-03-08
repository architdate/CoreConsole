using System;
using System.IO;
using System.Linq;
using CoreConsole.APIs;
using PKHeX.Core;

namespace CoreConsole
{
    class ConsoleIndex
    {
        public static PKM pk;
        static void Main(string[] args)
        {
            Initialize(args);
            if (args.Contains("-l"))
            {
                // Legality API calls
                var lc = new LegalityCheck(pk);
                if (args.Contains("--verbose")) Console.WriteLine(lc.VerboseReport);
                else Console.WriteLine(lc.Report);
            }
            if (args.Contains("-alm"))
            {
                if (!args.Contains("--version")) Console.WriteLine("Specify version with the [--version] tag");
                else
                {
                    var alm = new AutoLegality(pk, args[Array.IndexOf(args, "--version") + 1]);
                    if (alm != null)
                    {
                        string output = Util.CleanFileName(alm.GetLegalPKM().FileName);
                        File.WriteAllBytes($".\\output\\{output}", alm.GetLegalPKM().DecryptedBoxData);
                    }
                    else Console.WriteLine("Invalid version");
                }
            }
        }
        
        private static void Initialize(string[] args)
        {
            // check -i for input and get file path in the next arg
            string path = GetFilePath(args);
            byte[] data = File.ReadAllBytes(path);
            pk = PKMConverter.GetPKMfromBytes(data);
        }

        private static string GetFilePath(string[] args)
        {
            return args[Array.IndexOf(args, "-i") + 1];
        }
    }
}

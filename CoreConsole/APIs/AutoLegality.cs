using System;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace CoreConsole.APIs
{
    class AutoLegality
    {
        public static LegalityCheck lc;
        public static bool Legal => lc.Legal;
        public static PKM legalpk;
        private bool debug = true;

        public AutoLegality(PKM pk, string ver)
        {
            bool valid = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (valid)
                ProcessALM(pk, game);
            return;
        }

        public void ProcessALM(PKM pkm, GameVersion ver = GameVersion.GP)
        {
            lc = new LegalityCheck(pkm);
            if (Legal)
                legalpk = pkm;
            else
                legalpk = Legalize(pkm, ver);
        }

        public PKM Legalize(PKM pk, GameVersion ver)
        {
            if (debug) Console.WriteLine(lc.Report);
            var sav = SaveUtil.GetBlankSAV(ver, "PKHeX");
            var updated = sav.Legalize(pk);
            lc = new LegalityCheck(updated);
            if (Legal)
            {
                legalpk = updated;
                Console.WriteLine("====================================");
                Console.WriteLine("= Legalized with Auto Legality Mod =");
                Console.WriteLine("====================================");
                Console.WriteLine(lc.VerboseReport);
            }
            return updated;
        }

        public PKM GetLegalPKM()
        {
            return legalpk;
        }
    }
}

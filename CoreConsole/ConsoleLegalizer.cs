using System;
using System.Diagnostics;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace CoreConsole
{
    internal static class ConsoleLegalizer
    {
        public static PKM Legalize(PKM pk, GameVersion ver, LegalityAnalysis old = null)
        {
            if (old != null)
                Debug.WriteLine(old.Report());

            var sav = SaveUtil.GetBlankSAV(ver, "PKHeX");
            var updated = sav.Legalize(pk);
            var la = new LegalityAnalysis(updated);
            if (!la.Valid)
                return pk;

            Console.WriteLine("====================================");
            Console.WriteLine("= Legalized with Auto Legality Mod =");
            Console.WriteLine("====================================");
            Console.WriteLine(la.Report(true));
            return updated;
        }

        public static PKM GetLegalPKM(PKM pk, string ver)
        {
            var la = new LegalityAnalysis(pk);
            if (la.Valid)
                return pk;

            var parsed = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (!parsed)
                return pk;
            return Legalize(pk, game, la);
        }
    }
}

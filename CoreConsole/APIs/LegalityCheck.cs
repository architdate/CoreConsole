using PKHeX.Core;

namespace CoreConsole.APIs
{
    class LegalityCheck
    {
        private static PKM pk;
        private static LegalityAnalysis la;

        public LegalityCheck(PKM pkm)
        {
            pk = pkm;
            la = new LegalityAnalysis(pk);
        }

        public string VerboseReport => la.Report(true);
        public string Report => la.Report(false);
        public bool Legal => la.Valid;
    }
}

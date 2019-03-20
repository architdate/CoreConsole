using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace LegalASP.Controllers
{
    [Route("api/lc")]
    public class LegalCheck : Controller
    {
        // GET api/lc
        [HttpGet]
        public string Get() => "Hello!";

        // POST api/lc
        [HttpPost]
        public string Post()
        {
            if (Request.ContentType != "application/octet-stream")
                return string.Empty;
            if (Request.Body.Length == 0)
                return string.Empty;

            var ver = Request.Headers["Version"];
            if (string.IsNullOrWhiteSpace(ver))
                return string.Empty;

            var data = new byte[Request.Body.Length];
            using (var ms = new MemoryStream(data))
                Request.Body.CopyTo(ms);

            try
            {
                var pk = PKMConverter.GetPKMfromBytes(data);
                var la = new LegalityAnalysis(pk);
                return la.Report();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }

    [Route("api/legalizer")]
    public class PKSM : Controller
    {
        // GET api/legalizer
        [HttpGet]
        public string Get() => "PKSM";

        // POST api/legalizer/
        [HttpPost]
        public string Post()
        {
            if (Request.ContentType != "application/octet-stream")
                return string.Empty;
            if (Request.Body.Length == 0)
                return string.Empty;

            var ver = Request.Headers["Version"];
            if (string.IsNullOrWhiteSpace(ver))
                return string.Empty;

            var parsed = Enum.TryParse<GameVersion>(ver, true, out var game);
            if (!parsed)
                return string.Empty;

            var data = new byte[Request.Body.Length];
            using (var ms = new MemoryStream(data))
                Request.Body.CopyTo(ms);

            try
            {
                var pk = PKMConverter.GetPKMfromBytes(data);
                var la = new LegalityAnalysis(pk);
                if (la.Valid)
                    return string.Empty;
                var legalized = Legalize(pk, game, la);
                return Convert.ToBase64String(legalized.DecryptedBoxData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

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
    }
}

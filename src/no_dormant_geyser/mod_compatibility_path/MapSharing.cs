using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NoDormantGeyser
{
    /*
     *   temporary disable NoDormantGeyser mod when ths map sharing process begin
     *   1825374565 is id of map sharing mod
     *   https://steamcommunity.com/sharedfiles/filedetails/?id=1825374565
     */

    [HarmonyPatch]
    [HarmonyAfter("1825374565.steam")]
    public static class MapSharingPatch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethod(HarmonyInstance _)
        {
            var Utils = AccessTools.TypeByName("ToolsNotIncluded.ModCommon.CommonPatches+GeyserOnSpawn");

            if (Utils == null)
                yield break;

            var postfix = AccessTools.Method(Utils, "Postfix", new Type[] { typeof(Geyser) });

            yield return postfix;
        }

        [HarmonyPrefix]
        public static void Prefix()
        {
            NoDormantGeyserPatch.disabled = true;
        }

        [HarmonyPostfix]
        public static void Postfix()
        { 
            NoDormantGeyserPatch.disabled = false;
        }
    }
}

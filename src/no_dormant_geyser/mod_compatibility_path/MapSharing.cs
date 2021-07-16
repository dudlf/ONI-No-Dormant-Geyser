using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NoDormantGeyser
{
    /*
     *   https://steamcommunity.com/sharedfiles/filedetails/?id=1825374565
     */

/*    [HarmonyPatch]
    [HarmonyAfter("1825374565.steam")]*/
    public static class MapSharingPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var Utils = AccessTools.TypeByName("ToolsNotIncluded.ModCommon.CommonPatches.GeyserOnSpawn");
            if (Utils == null)
                yield break;

            var postfix = AccessTools.Method(Utils, "Postfix", new Type[] { typeof(Geyser) });

            yield return postfix;
        }

        /*[HarmonyPrefix]*/
        public static void Prefix()
        {
            NoDormantGeyserPatch.disabled = true;
        }

        /*[HarmonyPostfix]*/
        public static void Postfix()
        {
            NoDormantGeyserPatch.disabled = false;
        }   
    }
}

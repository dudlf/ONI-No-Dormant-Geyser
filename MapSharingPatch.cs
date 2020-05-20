using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace NoDormantGeyser
{
    [HarmonyPatch]
    [HarmonyAfter("1825374565.steam")]
    public static class MapSharingPatch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethod(HarmonyInstance harmony)
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
            MainPatch.disable = true;
        }

        [HarmonyPostfix]
        public static void Postfix()
        { 
            MainPatch.disable = false;
        }
    }
}

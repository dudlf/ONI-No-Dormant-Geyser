using Harmony;
using PeterHan.PLib.Options;
using System;
using System.Linq;

namespace NoDormantGeyser
{
    public static class MainPatch
    {
        public static bool disable = false;

        [HarmonyPatch(typeof(GeyserConfigurator.GeyserInstanceConfiguration))]
        [HarmonyPatch("GetYearPercent")]
        public static class YearPercentPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref object __instance, ref float __result)
            {
                var geyserType = ((GeyserConfigurator.GeyserInstanceConfiguration)__instance).geyserType;
                if (!disable && !Config.IsExcluded(geyserType))
                    __result = 1f;
            }
        }

        [HarmonyPatch(typeof(GeyserConfigurator.GeyserInstanceConfiguration))]
        [HarmonyPatch("GetMassPerCycle")]
        public static class MassPerCyclePatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref object __instance, ref float ___scaledYearPercent, ref float ___scaledRate, ref float __result)
            {
                var config = (GeyserConfigurator.GeyserInstanceConfiguration)__instance;

                if (!disable && !Config.IsExcluded(config) && (!GeyserCrackingPatch.disable.ContainsKey(config) || !GeyserCrackingPatch.disable[config]))
                {
                    __result = ___scaledRate * ___scaledYearPercent;
                }
            }
        }

        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class Init
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                disable = false;
            }
        }
    }
}

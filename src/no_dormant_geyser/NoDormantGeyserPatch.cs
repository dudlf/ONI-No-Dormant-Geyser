using Harmony;

namespace NoDormantGeyser
{
    public static class NoDormantGeyserPatch
    {
        public static bool disabled = false;

        [HarmonyPatch(typeof(GeyserConfigurator.GeyserInstanceConfiguration))]
        [HarmonyPatch("GetYearPercent")]
        public static class YearPercentPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref object __instance, ref float __result)
            {
                if (disabled)
                    return;

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
                var geyserConfig = (GeyserConfigurator.GeyserInstanceConfiguration)__instance;

                if (disabled || GeyserCrackingPatch.IsGeyserCracking(geyserConfig))
                    return;

                __result = ___scaledRate * ___scaledYearPercent;
            }
        }

        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class Init
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                disabled = false;
            }
        }
    }
}

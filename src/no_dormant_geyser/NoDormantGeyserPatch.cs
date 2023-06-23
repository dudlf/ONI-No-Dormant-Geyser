using HarmonyLib;
using UnityEngine;

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
            public static void Postfix(ref object __instance, ref float __result)
            {
                if (disabled)
                    return;

                if (__instance.GetType() != typeof(GeyserConfigurator.GeyserInstanceConfiguration))
                {
                    return;
                }

                GeyserConfigurator.GeyserInstanceConfiguration configuration = __instance as GeyserConfigurator.GeyserInstanceConfiguration;
                Geyser.GeyserModification modifier = configuration.GetModifier();

                float yearPercent = Mathf.Clamp(GetModifiedValue(configuration.scaledYearPercent, modifier.yearPercentageModifier, Geyser.yearPercentageModificationMethod), 0f, 1f);
                
                __result = __result * yearPercent;
            }
        }

        private static float GetModifiedValue(float geyserVariable, float modifier, Geyser.ModificationMethod method)
        {
            float num = geyserVariable;
            if (method != Geyser.ModificationMethod.Values)
            {
                if (method == Geyser.ModificationMethod.Percentages)
                {
                    num += geyserVariable * modifier;
                }
            }
            else
            {
                num += modifier;
            }
            return num;
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

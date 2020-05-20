using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace NoDormantGeyser
{
    [HarmonyPatch]
    [HarmonyAfter("1730968929.steam")]
    public static class GeyserCrackingPatch
    {
        private static FieldInfo emitter;
        public  static Dictionary<GeyserConfigurator.GeyserInstanceConfiguration, bool> disable;

        static GeyserCrackingPatch()
        {
            emitter = typeof(Geyser).GetField("emitter", BindingFlags.Instance | BindingFlags.NonPublic);

            disable = new Dictionary<GeyserConfigurator.GeyserInstanceConfiguration, bool>();
        }

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods(HarmonyInstance harmony)
        {
            var crackable = AccessTools.TypeByName("HexiGeyserCracking.Crackable");
            if (crackable == null)
                yield break;

            var oncracked = AccessTools.Method(crackable, "OnCracked");
            var onspawn = AccessTools.Method(crackable, "OnSpawn");

            yield return oncracked;
            yield return onspawn;
        }

        [HarmonyPrefix]
        public static void Prefix(ref Geyser ___geyser, ref float ___curP)
        {
            if (!disable.ContainsKey(___geyser.configuration))
            {
                disable.Add(___geyser.configuration, false);
            }
            if (___curP > 0f)
            {
                disable[___geyser.configuration] = true;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(MethodBase __originalMethod, ref Geyser ___geyser)
        {
            if (disable[___geyser.configuration])
            {
                disable[___geyser.configuration] = false;
                ((ElementEmitter)emitter.GetValue(___geyser)).outputElement.massGenerationRate = ___geyser.configuration.GetEmitRate();

                if (DetailsScreen.Instance != null && DetailsScreen.Instance.target == ___geyser.gameObject && SelectTool.Instance.selected != null)
                {
                    SelectTool.Instance.selected = ___geyser.GetComponent<KSelectable>();
                    DetailsScreen.Instance.Refresh(___geyser.gameObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    public class Init
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            GeyserCrackingPatch.disable.Clear();
        }
    }
}

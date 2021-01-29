using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace NoDormantGeyser
{
    /*
     *  temporary disable NoDormantGeyser mod when geyser cracked
     *  1730968929 is id of GeyserCracking mod
     *  https://steamcommunity.com/sharedfiles/filedetails/?id=1730968929
     */

    [HarmonyPatch]
    [HarmonyAfter("1730968929.steam")]
    public static class GeyserCrackingPatch
    {
        private static FieldInfo emitter;
        public  static SortedSet<GeyserConfigurator.GeyserInstanceConfiguration> crackedGeysers;

        static GeyserCrackingPatch()
        {
            emitter = typeof(Geyser).GetField("emitter", BindingFlags.Instance | BindingFlags.NonPublic);

            crackedGeysers = new SortedSet<GeyserConfigurator.GeyserInstanceConfiguration>();
        }

        // disable when geyser cracked and geyser spawned(when game loads)
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
            if (___curP > 0f)
            {
                MarkGeyserCracking(___geyser);
            }
        }

        [HarmonyPostfix]
        public static void Postfix(MethodBase __originalMethod, ref Geyser ___geyser)
        {
            if (!IsGeyserCracking(___geyser))
                return;

            // it must be called first
            UnmarkGeyserCracking(___geyser);

            ((ElementEmitter)emitter.GetValue(___geyser)).outputElement.massGenerationRate = ___geyser.configuration.GetEmitRate();

            // refresh geyser details screen
            if (DetailsScreen.Instance != null && DetailsScreen.Instance.target == ___geyser.gameObject && SelectTool.Instance.selected != null)
            {
                SelectTool.Instance.selected = ___geyser.GetComponent<KSelectable>();
                DetailsScreen.Instance.Refresh(___geyser.gameObject);
            }
        }

        public static bool IsGeyserCracking(GeyserConfigurator.GeyserInstanceConfiguration geyserConfig)
        {
            return crackedGeysers.Contains(geyserConfig);
        }

        public static bool IsGeyserCracking(Geyser geyser)
        {
            return crackedGeysers.Contains(geyser.configuration);
        }

        public static bool MarkGeyserCracking(Geyser geyser)
        {
            return crackedGeysers.Add(geyser.configuration);
        }

        public static bool UnmarkGeyserCracking(Geyser geyser)
        {
            return crackedGeysers.Remove(geyser.configuration);
        }
    }

    // clear geyser lists before game loads
    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    public class Init
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            GeyserCrackingPatch.crackedGeysers.Clear();
        }
    }
}

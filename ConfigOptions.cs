using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using System;
using Harmony;
using System.Linq;

namespace NoDormantGeyser
{
    [ModInfo("No Dormant Geyser", null, null, false)]
    [ConfigFile("config.json", true)]
    [Serializable]
    public class ConfigOptions : POptions.SingletonOptions<ConfigOptions>
    {
        [JsonProperty]
        [Option("Exclude", "Mod will ignore geysers in this list")]
        public string[] Excludes;
    }

    public static class Config
    {
        private static string[] ExcludeElement;

        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class OnGameInit
        {
            [HarmonyPrefix]
            public static void Prefix()
            { 
                ExcludeElement = POptions.SingletonOptions<ConfigOptions>.Instance.Excludes;
            }
        }

        public static void OnLoad()
        {
            PUtil.InitLibrary(true);
            POptions.RegisterOptions(typeof(ConfigOptions));
        }

        public static bool IsExcluded(in GeyserConfigurator.GeyserInstanceConfiguration configuration)
        {
            return IsExcluded(configuration.geyserType);
        }

        public static bool IsExcluded(in GeyserConfigurator.GeyserType geyserType)
        {
            return IsExcluded(geyserType.element);
        }

        public static bool IsExcluded(in SimHashes element)
        {
            return ExcludeElement.Contains(Enum.GetName(typeof(SimHashes), element));
        }

        public static bool IsExcluded(in string element)
        {
            return ExcludeElement.Contains(element);
        }
    }
}

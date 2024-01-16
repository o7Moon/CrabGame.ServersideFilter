using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;

namespace serversidefilter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        static ConfigEntry<string> replacement;
        static Plugin Instance;
        public override void Load()
        {
            Instance = this;
            replacement = Config.Bind<string>("Filter", "Replacement word", "muck", "the word used to replace profanity");
            Harmony.CreateAndPatchAll(typeof(Plugin));

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        public static void onSceneLoad(Scene scene, LoadSceneMode mode) {
            Instance.Config.Reload();
        }
        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.SendChatMessage))]
        [HarmonyPrefix]
        public static void sendMessageHook(ulong __0, ref string __1){
            foreach (string bad in ChatBox.Instance.field_Private_List_1_String_0) {
                __1 = Regex.Replace(__1, bad, replacement.Value, RegexOptions.IgnoreCase);
            }
        }
    }
}
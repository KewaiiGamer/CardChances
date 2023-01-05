using System;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CardChances
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Actual patching is just a one-liner!
            Harmony.CreateAndPatchAll(typeof(ScriptEditor_Patch));
        }
    }
    class ScriptEditor_Patch
    {
        [HarmonyPatch(typeof(Script_Editor), "SaveCurrentCard")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]
        static void SaveCurrentCard_Patch()
        {
            if (Mod_Manager.currentCustomCard == null)
                return;
            CustomCard currentCustomCard = Mod_Manager.currentCustomCard;
            
            foreach (CardValueKey cardValueKey in Script_Editor.variables.ToList())
            {
                if (cardValueKey.key.StartsWith("amount="))
                {
                    string cardAmount = cardValueKey.key.Split("=")[1];
                    currentCustomCard.cardAmount = Convert.ToInt16(cardAmount);
                }
            }
            Script_Editor.instance.customCardVisual.Setup(Mod_Manager.currentCustomCard, 0);
            Debug.Log("Saved card: " + currentCustomCard.cardAmount);
        }
    }
}

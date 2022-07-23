using BepInEx;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace DebugDeleteCard
{
    [BepInPlugin("de.benediktwerner.stacklands.debugdeletecard", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(WorldManager), "DebugUpdate")]
        [HarmonyPrefix]
        private static void WorldManager__DebugUpdate__Prefix(ref WorldManager __instance)
        {
            if (InputController.instance.GetKeyDown(Key.Delete) && __instance.HoveredCard != null)
            {
                Villager villager = __instance.HoveredCard.CardData as Villager;
                if (villager != null)
                {
                    villager.Damage(100);
                    return;
                }
                __instance.HoveredCard.DestroyCard(false, true);
            }
        }
    }
}

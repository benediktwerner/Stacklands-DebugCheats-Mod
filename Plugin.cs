using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace DebugCheats
{
    [BepInPlugin("de.benediktwerner.stacklands.debugcheats", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> DebugKeysEnabled;
        public static ConfigEntry<bool> DisableFood;
        public static ConfigEntry<bool> OverrideMonthLength;
        public static ConfigEntry<float> MonthLength;

        private void Awake()
        {
            Enabled = Config.Bind("General", "Enabled", true, "Can be used to disable the whole mod");
            DebugKeysEnabled = Config.Bind("General", "Debug Keys Enabled", true, "Enable additional debugging shortcuts");
            DisableFood = Config.Bind("General", "Disable Food", true, "Disable villager food requirements");
            OverrideMonthLength = Config.Bind("General", "Override Month Length", true, "Override month length with the value set in 'Month Length' below.");
            MonthLength = Config.Bind("General", "Month Length", float.PositiveInfinity, "How long months should be. Vanilla is 90 for Short, 120 for Normal, 200 for Long.");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(WorldManager), "Update")]
        [HarmonyPrefix]
        private static void WorldManager__Update(ref WorldManager __instance)
        {
            if (__instance.MonthTimer > __instance.MonthTime * 1.5) {
                __instance.MonthTimer = __instance.MonthTime / 2;
            }

            if (!Enabled.Value || !DebugKeysEnabled.Value) return;

            var card = __instance.HoveredCard;
            if ((InputController.instance.GetKeyDown(Key.Delete) || (InputController.instance.GetKey(Key.Delete) && InputController.instance.GetKey(Key.RightShift))) && card != null)
            {
                __instance.DestroyStack(card);
            }

            if (InputController.instance.GetKeyDown(Key.F1))
            {
                GameScreen.instance.DebugScreen.gameObject.SetActive(!GameScreen.instance.DebugScreen.gameObject.activeInHierarchy);
            }

            if (GameCanvas.instance.AboveMeOrMyChildren(GameScreen.instance.DebugScreen.rectTransform, InputController.instance.ClampedMousePosition()) && InputController.instance.InputString.Length > 0)
            {
                GameScreen.instance.JumpToDebugPosition(InputController.instance.InputString[0]);
            }

            if (InputController.instance.GetKeyDown(Key.F2))
            {
                var currency = __instance.CurrentBoard.Id == "main" ? "gold" : "shell";
                __instance.CreateCardStack(__instance.MiddleOfBoard(), 5, currency);
            }

            if (InputController.instance.GetKeyDown(Key.C) && card != null)
            {
                __instance.CreateCard(card.transform.position, card.CardData.Id);
            }
        }

        [HarmonyPatch(typeof(WorldManager), "GetRequiredFoodCount")]
        [HarmonyPrefix]
        private static void NoFood(ref bool __runOriginal, ref int __result)
        {
            if (!DisableFood.Value) return;
            __runOriginal = false;
            __result = 0;
        }

        [HarmonyPatch(typeof(WorldManager), "MonthTime", MethodType.Getter)]
        [HarmonyPrefix]
        private static void InfiniteMonths(ref bool __runOriginal, ref float __result)
        {
            if (!OverrideMonthLength.Value) return;
            __runOriginal = false;
            __result = MonthLength.Value;
        }
    }
}

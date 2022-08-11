using System;
using System.Collections.Generic;
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
        public static ConfigEntry<bool> DisableStarving;
        public static ConfigEntry<bool> DisableEating;
        public static ConfigEntry<bool> OverrideMonthLength;
        public static ConfigEntry<int> OverrideMaxCards;
        public static ConfigEntry<float> MonthLength;

        private void Awake()
        {
            Enabled = Config.Bind("General", "Enabled", true, "Can be used to disable the whole mod");
            DebugKeysEnabled = Config.Bind(
                "General",
                "Debug Keys Enabled",
                true,
                "Enable additional debugging shortcuts"
            );
            DisableStarving = Config.Bind(
                "General",
                "Disable Starving",
                false,
                "Villagers will still eat if there is food but they won't die if there is none."
            );
            DisableEating = Config.Bind(
                "General",
                "Disable Eating",
                true,
                "Villagers won't need food and won't eat any even if there is some."
            );
            OverrideMonthLength = Config.Bind(
                "General",
                "Override Month Length",
                true,
                "Override month length with the value set in 'Month Length' below."
            );
            OverrideMaxCards = Config.Bind(
                "General",
                "Override Max Cards",
                500,
                "Override maximum number of cards. Set to -1 to use the game's default calculation."
            );
            MonthLength = Config.Bind(
                "General",
                "Month Length",
                float.PositiveInfinity,
                "How long months should be. Vanilla is 90 for Short, 120 for Normal, 200 for Long."
            );

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        private static bool HoldingShift()
        {
            return InputController.instance.GetKey(Key.LeftShift) || InputController.instance.GetKey(Key.RightShift);
        }

        [HarmonyPatch(typeof(WorldManager), "Update")]
        [HarmonyPrefix]
        private static void WorldManager__Update(ref WorldManager __instance)
        {
            if (__instance.MonthTimer > __instance.MonthTime * 1.5)
            {
                __instance.MonthTimer = __instance.MonthTime / 2;
            }

            if (!Enabled.Value || !DebugKeysEnabled.Value)
                return;

            var card = __instance.HoveredCard;
            if (
                (
                    InputController.instance.GetKeyDown(Key.Delete)
                    || (InputController.instance.GetKey(Key.Delete) && HoldingShift())
                )
                && card != null
            )
            {
                __instance.DestroyStack(card);
            }

            if (InputController.instance.GetKeyDown(Key.F1))
            {
                GameScreen.instance.DebugScreen.gameObject.SetActive(
                    !GameScreen.instance.DebugScreen.gameObject.activeInHierarchy
                );
            }

            if (
                GameCanvas.instance.AboveMeOrMyChildren(
                    GameScreen.instance.DebugScreen.rectTransform,
                    InputController.instance.ClampedMousePosition()
                )
                && InputController.instance.InputString.Length > 0
            )
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
                if (HoldingShift())
                {
                    var pos = card.transform.position;
                    var cards = new List<string>();
                    do
                    {
                        cards.Add(card.CardData.Id);
                        card = card.Child;
                    } while (card != null);
                    var parent = __instance.CreateCard(pos, cards[0]).MyGameCard;
                    foreach (var id in cards.GetRange(1, cards.Count - 1))
                    {
                        var child = __instance.CreateCard(pos, id, checkAddToStack: false).MyGameCard;
                        parent.Child = child;
                        child.Parent = parent;
                        parent = child;
                    }
                }
                else
                {
                    __instance.CreateCard(card.transform.position, card.CardData.Id);
                }
            }
        }

        [HarmonyPatch(typeof(WorldManager), "GetRequiredFoodCount")]
        [HarmonyPrefix]
        private static void NoFood(ref bool __runOriginal, ref int __result)
        {
            if (!Enabled.Value || !DisableStarving.Value)
                return;
            __runOriginal = false;
            __result = 0;
        }

        [HarmonyPatch(typeof(WorldManager), "GetCardRequiredFoodCount")]
        [HarmonyPrefix]
        private static void NoEating(ref bool __runOriginal, ref int __result)
        {
            if (!Enabled.Value || !DisableEating.Value)
                return;
            __runOriginal = false;
            __result = 0;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldManager), "GetMaxCardCount")]
        [HarmonyPatch(new Type[0])]
        [HarmonyPatch(new Type[] { typeof(GameBoard) })]
        private static void MaxCards(ref bool __runOriginal, ref int __result)
        {
            if (!Enabled.Value || OverrideMaxCards.Value < 0)
                return;
            __runOriginal = false;
            __result = OverrideMaxCards.Value;
        }

        [HarmonyPatch(typeof(WorldManager), "MonthTime", MethodType.Getter)]
        [HarmonyPrefix]
        private static void InfiniteMonths(ref bool __runOriginal, ref float __result)
        {
            if (!Enabled.Value || !OverrideMonthLength.Value)
                return;
            __runOriginal = false;
            __result = MonthLength.Value;
        }
    }
}

using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace DebugCheats
{
    [BepInPlugin("de.benediktwerner.stacklands.debugcheats", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource L;

        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> DebugKeysEnabled;
        public static ConfigEntry<int> OverrideMaxCards;
        public static ConfigEntry<bool> DisableEating;
        public static ConfigEntry<bool> InfiniteMonths;

        private void Awake()
        {
            L = Logger;
            Enabled = Config.Bind("General", "Enabled", true, "Can be used to disable the whole mod");
            DebugKeysEnabled = Config.Bind(
                "General",
                "Debug Keys Enabled",
                true,
                "Enable additional debugging shortcuts"
            );
            OverrideMaxCards = Config.Bind(
                "General",
                "Override Max Cards",
                999,
                "Override maximum number of cards. Set to -1 to use the game's default calculation."
            );
            DisableEating = Config.Bind(
                "General",
                "Disable Eating",
                true,
                "Same as the option in the game's debug menu but stored persistently."
            );
            InfiniteMonths = Config.Bind(
                "General",
                "Infinite Months",
                true,
                "Same as the option in the game's debug menu but stored persistently."
            );

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        private static bool HoldingShift()
        {
            return InputController.instance.GetKey(Key.LeftShift) || InputController.instance.GetKey(Key.RightShift);
        }

        [HarmonyPatch(typeof(DebugScreen), nameof(DebugScreen.ToggleEndlessMoon))]
        [HarmonyPostfix]
        private static void SyncEndlessMoon()
        {
            InfiniteMonths.Value = WorldManager.instance.DebugEndlessMoonEnabled;
        }

        [HarmonyPatch(typeof(DebugScreen), nameof(DebugScreen.ToggleNoFood))]
        [HarmonyPostfix]
        private static void SyncToggleNoFood()
        {
            DisableEating.Value = WorldManager.instance.DebugNoFoodEnabled;
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Update))]
        [HarmonyPrefix]
        private static void WorldManager__Update(ref WorldManager __instance)
        {
            if (!Enabled.Value)
                return;

            __instance.DebugNoFoodEnabled = DisableEating.Value;
            __instance.DebugEndlessMoonEnabled = InfiniteMonths.Value;

            if (!DebugKeysEnabled.Value)
                return;

            var card = __instance.HoveredCard;
            if (
                (
                    InputController.instance.GetKeyDown(Key.Delete)
                    || (
                        (InputController.instance.GetKey(Key.R) || InputController.instance.GetKey(Key.Delete))
                        && HoldingShift()
                    )
                )
            )
            {
                if (card != null)
                {
                    if (card.CardData is Combatable c && c.MyConflict != null)
                        c.MyConflict.LeaveConflict(c);
                    if (card.Parent != null)
                    {
                        card.Parent.Child = null;
                        card.Parent = null;
                    }
                    __instance.DestroyStack(card);
                }
                else if (__instance.HoveredDraggable != null && __instance.HoveredDraggable is Boosterpack b)
                    UnityEngine.Object.Destroy(b.gameObject);
            }

            if (InputController.instance.GetKeyDown(Key.F1))
            {
                GameScreen.instance.DebugScreen.gameObject.SetActive(
                    !GameScreen.instance.DebugScreen.gameObject.activeInHierarchy
                );
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
    }
}

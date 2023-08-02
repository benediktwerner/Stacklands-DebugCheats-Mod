using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace DebugCheats
{
    public class Plugin : Mod
    {
        public static ModLogger L;

        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> DebugKeysEnabled;
        public static ConfigEntry<int> OverrideMaxCards;
        public static ConfigEntry<bool> DisableEating;
        public static ConfigEntry<bool> InfiniteMonths;
        public static ConfigEntry<bool> DisableGameOver;
        public static ConfigEntry<string> DebugKey;

        public static Key DebugHotkey = Key.F1;

        public static ConfigFile ConfigFile;

        private ConfigEntry<T> CreateConfig<T>(string name, T defaultValue, string description)
        {
            return Config.GetEntry<T>(name, defaultValue, new ConfigUI { Tooltip = description });
        }

        private void Awake()
        {
            L = Logger;
            Enabled = CreateConfig("Enabled", true, "Can be used to disable the whole mod");
            DebugKey = CreateConfig("Debug Menu Shortcut", "F1", "Key to toggle the debug menu.");
            SetDebugHotkey(DebugKey.Value);
            DebugKey.OnChanged += SetDebugHotkey;
            DebugKeysEnabled = CreateConfig("Debug Keys Enabled", true, "Enable additional debugging shortcuts");
            OverrideMaxCards = CreateConfig(
                "Override Max Cards",
                999,
                "Override maximum number of cards. Set to -1 to use the game's default calculation."
            );
            DisableEating = CreateConfig(
                "Disable Eating",
                true,
                "Same as the option in the game's debug menu but stored persistently."
            );
            InfiniteMonths = CreateConfig(
                "Infinite Months",
                true,
                "Same as the option in the game's debug menu but stored persistently."
            );
            DisableGameOver = CreateConfig(
                "Disable Game Over",
                true,
                "Prevents game over when all villagers are dead."
            );

            Harmony.PatchAll(typeof(Plugin));
            ConfigFile = Config;
        }

        private static void SetDebugHotkey(string newValue)
        {
            Key? maybeKey = Keyboard.current.allKeys.FirstOrDefault(x => x.displayName == newValue)?.keyCode;
            if (maybeKey is Key key)
            {
                DebugHotkey = key;
            }
            else
                L.LogWarning($"Invalid hotkey: {newValue}");
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
            ConfigFile.Save();
        }

        [HarmonyPatch(typeof(DebugScreen), nameof(DebugScreen.ToggleNoFood))]
        [HarmonyPostfix]
        private static void SyncToggleNoFood()
        {
            DisableEating.Value = WorldManager.instance.DebugNoFoodEnabled;
            ConfigFile.Save();
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

            if (InputController.instance.GetKeyDown(DebugHotkey))
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
                    var newCard = __instance.CreateCard(card.transform.position, card.CardData.Id);
                    if (newCard is Combatable c)
                    {
                        foreach (var equip in card.CardData.GetAllEquipables())
                            c.CreateAndEquipCard(equip.Id, false);
                        c.HealthPoints = (card.CardData as Combatable).HealthPoints;
                    }
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.CheckDebugInput))]
        public static IEnumerable<CodeInstruction> BlockVanillaDebugCopy(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)17),
                    new CodeMatch(
                        OpCodes.Callvirt,
                        AccessTools.Method(typeof(InputController), nameof(InputController.GetKeyDown))
                    ),
                    new CodeMatch(OpCodes.Brfalse)
                )
                .ThrowIfInvalid("Didn't find vanilla debug copy")
                .Insert(
                    Transpilers.EmitDelegate<Func<bool>>(() => !Enabled.Value || !DebugKeysEnabled.Value),
                    new CodeInstruction(OpCodes.And)
                )
                .InstructionEnumeration();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Equipable), nameof(Equipable.CanBeDragged), MethodType.Getter)]
        [HarmonyPatch(typeof(StrangePortal), nameof(StrangePortal.CanBeDragged), MethodType.Getter)]
        [HarmonyPatch(typeof(PirateBoat), nameof(PirateBoat.CanBeDragged), MethodType.Getter)]
        [HarmonyPatch(typeof(Mob), nameof(PirateBoat.CanBeDragged), MethodType.Getter)]
        public static void AllowDraggingEverything(out bool __runOriginal, out bool __result)
        {
            __runOriginal = !Enabled.Value;
            __result = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mob), nameof(Mob.CanHaveCard))]
        public static void AllowEquippingEnemies(CardData otherCard, out bool __runOriginal, out bool __result)
        {
            __result = true;
            __runOriginal = !Enabled.Value || otherCard is not Equipable;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.CheckAllVillagersDead))]
        public static void PreventGameOver(out bool __runOriginal, out bool __result)
        {
            __result = false;
            __runOriginal = !Enabled.Value || !DisableGameOver.Value;
        }
    }
}

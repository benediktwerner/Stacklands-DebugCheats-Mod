# Stacklands DebugCheats Mod

Adds various (configurable) debug cheats to make testing stuff easier:

- Month never ends (can also be changed to a custom length instead)
- Villagers don't need food anymore (by default, they also won't consume any food but this can be changed so they still eat but won't starve anymore if there's no food)
- Press F1 to open the card spawn menu (instead of K+O+F1 in vanilla)
- When hovering over the card spawn menu, press a letter to jump to cards starting with that letter
- Press F2 to spawn 5 coins
- Press C while hovering over a card to duplicate it
- Press Shift+C while hovering over a card to duplicate it and its children
- Press Delete to delete the card below the cursor and its children
- Hold Shift+Delete to continuously delete cards the cursor touches
- Override max number of cards (default is 500)

To configure or disable the cheats, edit the `BepInEx/config/de.benediktwerner.stacklands.debugcheats.cfg` file which appears after starting the game once.

See also the HigherSidebar mod to increase the height of the card spawn menu.

## Manual Installation
This mod requires BepInEx to work. BepInEx is a modding framework which allows multiple mods to be loaded.

1. Download and install BepInEx from the [Thunderstore](https://stacklands.thunderstore.io/package/BepInEx/BepInExPack_Stacklands/).
4. Download this mod and extract it into `BepInEx/plugins/`
5. Launch the game

## Links
- Github: https://github.com/benediktwerner/Stacklands-DebugCheats-Mod
- Thunderstore: https://stacklands.thunderstore.io/package/benediktwerner/DebugCheats
- Nexusmods: https://www.nexusmods.com/stacklands/mods/8

## Changelog

- v1.2.1: Fix infinite loop when using Shift+C
- v1.2
  - Fix month timer when disabling infinite months or reducing the month time after playing with it for a while
  - Add Shift+C to copy a whole stack
  - Add option to override max card count
- v1.1: Make F2 spawn shells instead of coins when on the island
- v1.0: Initial release

# Stacklands DebugCheats Mod

Adds various (configurable) debug cheats to make testing stuff easier:

- Press F1 to open the card spawn menu (instead of K+O+F1 in vanilla)
- Press F2 to spawn 5 coins
- Press C while hovering over a card to duplicate it
- Press Shift+C while hovering over a card to duplicate it and its children
- Press Delete to delete the card below the cursor and its children
- Hold Shift+Delete to continuously delete cards the cursor touches
- Override max number of cards (default is 500)
- Option for infinite months
- Option to disalbe food usage

To configure or disable the cheats, edit the `BepInEx/config/de.benediktwerner.stacklands.debugcheats.cfg` file which appears after starting the game once.

See also the HigherSidebar mod to increase the height of the card spawn menu.

## Manual Installation
This mod requires BepInEx to work. BepInEx is a modding framework which allows multiple mods to be loaded.

1. Download and install BepInEx from the [Thunderstore](https://stacklands.thunderstore.io/package/BepInEx/BepInExPack_Stacklands/).
4. Download this mod and extract it into `BepInEx/plugins/`
5. Launch the game

## Development
1. Install BepInEx
2. This mod uses publicized game DLLs to get private members without reflection
   - Use https://github.com/CabbageCrow/AssemblyPublicizer for example to publicize `Stacklands/Stacklands_Data/Managed/GameScripts.dll` (just drag the DLL onto the publicizer exe)
   - This outputs to `Stacklands_Data\Managed\publicized_assemblies\GameScripts_publicized.dll` (if you use another publicizer, place the result there)
3. Compile the project. This copies the resulting DLL into `<GAME_PATH>/BepInEx/plugins/`.
   - Your `GAME_PATH` should automatically be detected. If it isn't, you can manually set it in the `.csproj` file.
   - If you're using VSCode, the `.vscode/tasks.json` file should make it so that you can just do `Run Build`/`Ctrl+Shift+B` to build.

## Links
- Github: https://github.com/benediktwerner/Stacklands-DebugCheats-Mod
- Thunderstore: https://stacklands.thunderstore.io/package/benediktwerner/DebugCheats
- Nexusmods: https://www.nexusmods.com/stacklands/mods/8

## Changelog

- v1.3.3:
  - Allow deleting cards with Shift+R (you can still use plain R for the game's built-in deletion)
  - Allow deleting booster packs
- v1.3.2: Sync infinite months and no food settings with game's debug screen
- v1.3.1: Add back infinite months and no food options since the game's options aren't persistent
- v1.3.0: Witch Forest Update
  - Fix incompatibility with the game's Witch Forest Update
  - Remove infinite months and food blocker (now built into the game's debug menu)
- v1.2.1: Fix infinite loop when using Shift+C
- v1.2
  - Fix month timer when disabling infinite months or reducing the month time after playing with it for a while
  - Add Shift+C to copy a whole stack
  - Add option to override max card count
- v1.1: Make F2 spawn shells instead of coins when on the island
- v1.0: Initial release

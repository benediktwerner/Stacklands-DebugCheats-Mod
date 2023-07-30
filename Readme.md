# Stacklands DebugCheats Mod

Adds various (configurable) debug cheats to make testing stuff easier:

- Press F1 to open the card spawn menu (instead of K+O+F1 in vanilla)
- Press F2 to spawn 5 coins
- Press C while hovering over a card to duplicate it
- Press Shift+C while hovering over a card to duplicate it and its children
- Press Delete to delete the card or boosterpack below the cursor and its children
- Hold Shift+Delete or Shift+R to continuously delete cards or boosterpacks the cursor touches
- Override max number of cards (default is 500)
- Option for infinite months (synced with the option in the vanilla debug menu but persists over restarts)
- Option to disable food usage (synced with the option in the vanilla debug menu but persists over restarts)
- Option to disable game over when all villagers are dead
- Allows modifying the equipment of enemies
- Allows dragging enemies, portals, and pirate boats

## Development

- Build using `dotnet build`
- For release builds, add `-c Release`
- If you're using VSCode, the `.vscode/tasks.json` file allows building via `Run Build`/`Ctrl+Shift+B`

## Links

- Github: https://github.com/benediktwerner/Stacklands-DebugCheats-Mod
- Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3012068711

## Changelog

- v1.6.1: Fix debug settings not getting persisted when being toggled via in-game debug menu
- v1.6: Steam Workshop Support
- v1.5:
  - Allow dragging enemies
  - Add option to disable game over
- v1.4:
  - Copy equipment and health when copying single card
  - Block vanilla debug copying
  - Allow managing enemy equipment
- v1.3.6: Fix continuous deleting after a booster pack
- v1.3.5: Remove cards from combat and stack before deleting
- v1.3.4: Make Shift+R continuously delete while held down
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

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

## Possible keyboard hotkey values

You can configure the debug menu hotkey in the settings. Unfortunately, the game currently doesn't have a proper way to set hotkeys for mods so you have to enter the name of the hotkey you want to use. Valid values are:

Space, Enter, Tab, Backquote, Quote, Semicolon, Comma, Period, Slash, Backslash, LeftBracket, RightBracket, Minus, Equals, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9, Digit0, LeftShift, RightShift, LeftAlt, RightAlt, AltGr, LeftCtrl, RightCtrl, LeftMeta, RightMeta, LeftWindows, RightWindows, LeftApple, RightApple, LeftCommand, RightCommand, ContextMenu, Escape, LeftArrow, RightArrow, UpArrow, DownArrow, Backspace, PageDown, PageUp, Home, End, Insert, Delete, CapsLock, NumLock, PrintScreen, ScrollLock, Pause, NumpadEnter, NumpadDivide, NumpadMultiply, NumpadPlus, NumpadMinus, NumpadPeriod, NumpadEquals, Numpad0, Numpad1, Numpad2, Numpad3, Numpad4, Numpad5, Numpad6, Numpad7, Numpad8, Numpad9, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, OEM1, OEM2, OEM3, OEM4, OEM5, IMESelected

## Development

- Build using `dotnet build`
- For release builds, add `-c Release`
- If you're using VSCode, the `.vscode/tasks.json` file allows building via `Run Build`/`Ctrl+Shift+B`

## Links

- Github: https://github.com/benediktwerner/Stacklands-DebugCheats-Mod
- Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3012068711

## Changelog

- v1.6.4: Fix "F5 to hide UI" not working if the debug menu is opened via F1 instead of K+O+F1
- v1.6.3: Fix debug screen tabs getting cut off (thanks a lot @lopidav for the fix)
- v1.6.2: Allow configuring the debug menu hotkey
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

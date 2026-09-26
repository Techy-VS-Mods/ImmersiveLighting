# Changelog

## 1.1.0
- **Fixes lamps sharing state.** Fuel level, lit state and wick height were stored on the shared block object, so every placed
  lamp of the same variant showed the fuel level of whichever lamp updated last, and the interaction hints and the lit/unlit
  mesh could be wrong for the same reason. All per-lamp state now lives on each lamp's own block entity and is read by
  position. Two lamps with different fuel now each show their own level.
- Behaviour change for add-ons: `BlockLamp` no longer has the `HasFuel`, `Lit`, `Filled`, `RemainingFuel` and `WickHeight`
  fields. Read them from `BlockEntityLamp` instead (they are now read-only properties there).
- Added a regression test (two lamps with different fuel) to the headless test harness.

## 1.0.3
- Updated for Vintage Story 1.22.x (.NET 10).
- Lamp light is now stored per lamp and refreshed whenever the lamp is lit/extinguished, the wick height changes,
  or the fuel runs out, forcing a relight instead of waiting for a block to be placed or changed.
- Client re-applies the light when the lamp's state arrives from the server.

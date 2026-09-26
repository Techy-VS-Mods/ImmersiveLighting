# Changelog

## 1.1.0
- **Fixes lamps sharing state.** Fuel level, lit state and wick height were stored on the shared block object, so every placed
  lamp of the same variant showed the fuel level of whichever lamp updated last, and the interaction hints and the lit/unlit
  mesh could be wrong for the same reason. All per-lamp state now lives on each lamp's own block entity and is read by
  position. Two lamps with different fuel now each show their own level.
- Behaviour change for add-ons: `BlockLamp` no longer has the `HasFuel`, `Lit`, `Filled`, `RemainingFuel` and `WickHeight`
  fields. Read them from `BlockEntityLamp` instead (they are now read-only properties there).
- **More fuels.** The lamp now burns more than aqua vitae. Compatibility patches (each gated with `dependsOn`, so they only apply
  when the mod is installed) add burn properties to true liquid fuels: base-game olive and flax oil and spirit, Expanded Foods
  cooking oils, lard and strong/potent spirits, Dairy Plus ghee, and Biodiversity avocado/engkala oil and fruit/potato spirits.
- **Rebalanced burn times and flame temperatures.** Aqua vitae is now 1100 (was 700) at the same 50 min per litre baseline.
  Other spirits burn as fast, oils 2.25-2.5x as long, ghee and lard 3x as long, with cooler flames for thicker fuels.
- Added tests to the headless harness: a two-lamp regression check, and a fuel check that every installed liquid fuel variant
  has burn properties and is accepted by the lamp (120 of 120 pass with all compat mods installed; no patch errors either way).

## 1.0.3
- Updated for Vintage Story 1.22.x (.NET 10).
- Lamp light is now stored per lamp and refreshed whenever the lamp is lit/extinguished, the wick height changes,
  or the fuel runs out, forcing a relight instead of waiting for a block to be placed or changed.
- Client re-applies the light when the lamp's state arrives from the server.

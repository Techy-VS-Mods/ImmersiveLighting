# Changelog

## 1.2.0-rc.1 (release candidate)
- **Flame colour per fuel.** Each fuel burns the colour it would in real life: aqua vitae and potent spirits a pale blue, strong
  spirits blue-white, plain and fruit spirits near white (impurities tint the blue flame), olive and avocado oil golden yellow, seed
  and nut oils yellow-orange, linseed orange, ghee warm yellow, lard a dull orange-red.
- **Brightness follows how the fuel burns.** Soot makes flames glow, so clean spirit flames are dim and oils are bright. Brightness
  = wick level (5/10/20) x fuel luminosity x (1 - half the smoke), so a smoky fuel loses some of its light. Turning the wick up
  smokes more, as in a real lamp.
- **Smoke.** Lit lamps release smoke particles scaled by the fuel's smoke value and the wick height.
- Per-fuel values live in the patches (`attributes.immersivelighting`: `flameHue` 0-63, `flameSat` 0-7, `luminosity`, `smoke`).
  Fuels from mods we do not patch fall back to a warm, mildly smoky default. `burnTemperature` is flavour only.
- **Lamp readout.** Looking at a lamp shows its fuel, flame colour and smoke, and how long the fuel will last at the current wick
  ("Burning: about 50 min of fuel left at this wick").
- **Lighting needs an ignition source.** A lamp is lit with a firestarter or a lit torch (the game's own ignition system, so other
  mods' igniters work too, and the interaction hint shows what can light it). Dousing and wick changes stay bare-handed. Can be
  turned off (see settings).
- **Server settings** in `ModConfig/immersivelighting-server.json`: `BurnRateScale`, `BrightnessScale`, `SmokeScale`,
  `RequireIgnitionSource`. They are sent to every client, so server and clients always agree. Client setting `SmokeScale` in
  `ModConfig/immersivelighting.json` scales smoke in your own game only.
- **Handbook page** listing the fuels, with their flame, brightness, smoke and burn times.
- Tests: the headless harness checks every fuel's colour and brightness at each wick height, the resulting world light, and the
  config, ignition and readout behaviour.

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

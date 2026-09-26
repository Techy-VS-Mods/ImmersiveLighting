# Changelog

## 1.2.0-rc.1 (release candidate)
Everything since 1.0.2, the last release (the interim 1.0.3 and 1.1.0 builds were never released).

**Updated for Vintage Story 1.22**
- Ported to 1.22 (.NET 10). Requires game version 1.22.0 or newer.

**Fixes**
- Lamps no longer share state. Fuel level, lit state and wick height were stored on the block shared by every lamp of the same
  variant, so every placed lamp showed the fuel level of whichever one updated last, and the interaction hints and lit/unlit look
  could be wrong too. Each lamp now keeps its own state.
- Lamp light now updates when you light or douse the lamp, change the wick, or when the fuel runs out. The game only recalculates
  block light when a block changes, so a lamp could keep its old light; it now forces the recalculation, clearing the old light
  first when it dims.

**More fuels**
- Burn properties added (each patch only applies when its mod is installed, nothing required) to real liquid fuels from the base
  game (olive and flax oil, spirit), Expanded Foods (cooking oils, lard, strong and potent spirits), Dairy Plus (ghee) and
  Biodiversity (avocado and engkala oil, fruit and potato spirits).
- Rebalanced burn times: a litre at the lowest wick lasts about 50 min for aqua vitae and spirits, 113 min for flax and seed/nut
  oils, 125 min for olive, avocado and engkala oil, 150 min for ghee and lard. Aqua vitae flame temperature is now 1100 (was 700).

**Flame colour, brightness and smoke**
- Each fuel burns the colour it would in real life: aqua vitae and potent spirits pale blue, strong spirits blue-white, plain and
  fruit spirits near white, olive and avocado oil golden yellow, seed and nut oils yellow-orange, linseed orange, ghee warm yellow,
  lard dull orange-red.
- Brightness follows how the fuel burns: soot makes flames glow, so clean spirit flames are dim and oils are bright. Brightness =
  wick level (5/10/20) x fuel luminosity x (1 - half the smoke). Turning the wick up smokes more, as in a real lamp.
- Lit lamps release smoke particles scaled by the fuel's smoke value and the wick height.
- Per-fuel values live in the patches (`attributes.immersivelighting`: `flameHue` 0-63, `flameSat` 0-7, `luminosity`, `smoke`).
  Fuels from mods we do not patch fall back to a warm, mildly smoky default. `burnTemperature` is flavour only.

**Readout, lighting and settings**
- Looking at a lamp shows its fuel, flame colour and smoke, and how long the fuel will last at the current wick.
- A lamp is lit with a firestarter or a lit torch (the game's own ignition system, so other mods' igniters work too). Dousing and
  wick changes stay bare-handed. Can be turned off.
- Server settings in `ModConfig/immersivelighting-server.json`: `BurnRateScale`, `BrightnessScale`, `SmokeScale`,
  `RequireIgnitionSource`, sent to every client so server and clients agree. Client `SmokeScale` in
  `ModConfig/immersivelighting.json` scales smoke in your own game only.
- Handbook page "Lamp Fuels".

**For mod authors**
- `BlockLamp` no longer has the `HasFuel`, `Lit`, `Filled`, `RemainingFuel` and `WickHeight` fields. Read them from
  `BlockEntityLamp` (read-only properties).

**Tests**
- A headless server harness (`test/`) checks the light through every lamp state, that two lamps keep separate state, every
  installed fuel's colour and brightness at each wick height, and the config, ignition and readout behaviour.

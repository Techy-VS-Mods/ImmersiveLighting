## Immersive Lighting Mod
#### Version 1.2.0-rc.1 (release candidate)

## Description
An attempt to make more immersive lighting sources for Vintage Story. 

## Current Features

### Liquid Fuel Lamps
- Burn any combustible liquid at a rate determined by the liquid's burnDuration property
- Visually see the fuel level in the lamp
- Interactions to increase / decrease light & fuel consumption levels by adjusting wick height
- Can contain any liquid but will only light when a combustible liquid is present.

### Liquid Fuels
Any liquid with burn properties works in the lamp. This mod adds burn properties to a range of oils, fats and spirits so they can
be poured in and burnt. A litre (100 portions) lasts the times below at the lowest wick; each wick step uses one more portion per tick, so
wick 3 burns three times faster. Thin, volatile spirits burn quickest; thicker oils and fats are drawn up the wick more slowly.

| Fuel | Burn time per litre (wick 1) | Flame |
|---|---|---|
| Aqua vitae, spirits (base game, Expanded Foods strong/potent spirits, Biodiversity fruit and potato spirits) | ~50 min | 1000-1100 |
| Linseed (flax) and seed/nut oils: sunflower, peanut, soy, rice, seed | ~113 min | 850 |
| Olive oil, avocado oil, engkala oil | ~125 min | 900 |
| Ghee (Dairy Plus), liquid lard (Expanded Foods) | ~150 min | 750-800 |

#### Flame colour, brightness and smoke
Each fuel has its own flame colour, luminosity and smoke (`attributes.immersivelighting` on the fuel item, added by the patches).
Clean spirits burn a dim, pale blue; oils and fats burn brighter and yellower but smoke more, and a smokier flame loses some light.
Turning the wick up increases smoke. Tune brightness globally with `brightnessScale` in the lamp block's attributes, and turn lamp
smoke down or off with `SmokeScale` in `ModConfig/immersivelighting.json`.

#### Lighting, readout and settings
- Light a lamp by holding a firestarter or a lit torch against it (the game's own ignition system). Dousing and wick changes need no tool.
- Look at a lamp to see its fuel, flame colour, smoke and how long the fuel lasts at the current wick.
- Server settings in `ModConfig/immersivelighting-server.json`, sent to all clients: `BurnRateScale` (2 = fuel burns twice as fast),
  `BrightnessScale`, `SmokeScale` (0 = no smoke) and `RequireIgnitionSource` (false lets lamps be lit bare-handed).
- Client setting `SmokeScale` in `ModConfig/immersivelighting.json` scales smoke for just your own game.

Compatibility patches are per mod and only apply when that mod is installed (`dependsOn`), so nothing warns when a mod is absent:
base game, Expanded Foods, Dairy Plus and Biodiversity (orchard and crops).

## AI Usage
I use Claude, an AI assistant from Anthropic, for rapid development. I remain in sole control of the code and the direction of this mod. I am a developer with over 15 years of experience, and I provide the direction and guidance rather than letting the AI do all the thinking.

## Special Thanks to the following individuals on Discord for their support
- Pizza2000
- Dana
- ImNuts42
- Nat
- spearandfang 
- lazylion93
- & Many others who talk in the Discord mod-development channel
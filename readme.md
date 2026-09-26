## Immersive Lighting Mod
#### Version 1.1.0

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

Compatibility patches are per mod and only apply when that mod is installed (`dependsOn`), so nothing warns when a mod is absent:
base game, Expanded Foods, Dairy Plus and Biodiversity (orchard and crops).

## Special Thanks to the following individuals on Discord for their support
- Pizza2000
- Dana
- ImNuts42
- Nat
- spearandfang 
- lazylion93
- & Many others who talk in the Discord mod-development channel
# Changelog

## 1.0.3
- Updated for Vintage Story 1.22.x (.NET 10).
- Lamp light is now stored per lamp and refreshed whenever the lamp is lit/extinguished, the wick height changes,
  or the fuel runs out, forcing a relight instead of waiting for a block to be placed or changed.
- Client re-applies the light when the lamp's state arrives from the server.

# VS 1.22.7 block light: how it works and how to force a relight

Source: ilspycmd decompile of VintagestoryLib/API/VSSurvivalMod/VSEssentials 1.22.7 (in /tmp/lightres). Read-only research; nothing run on the server.

## 1. Pipeline and triggers
- Server: `ServerWorldMap.UpdateLighting(oldId,newId,pos)` enqueues an `UpdateLightingTask`; `ServerSystemRelight` drains it on a separate thread and calls `ChunkIlluminator.RemoveBlockLight` / `PlaceBlockLight` / sunlight, then marks chunks modified.
- Client mirrors this: `ClientWorldMap.UpdateLighting` -> `ClientSystemRelight` -> its own `ChunkIlluminator` -> `SetChunkDirty`.
- Only triggers: block set/exchange through an accessor with relight=true (server `world.BlockAccessor` and client `BlockAccessor` both are), `IBlockAccessor.RemoveBlockLight`, `MarkAbsorptionChanged`, `FullRelight`, chunk load/worldgen. Block entity state changes trigger nothing.

## 2. Where light is read
- `Block.GetLightHsv(IBlockAccessor, BlockPos, ItemStack=null)` (virtual, default returns the `LightHsv` field). Call sites: ServerSystemRelight.cs:83-84 and ClientSystemRelight.cs:80-81 (both the OLD and NEW block, evaluated at processing time, background thread), ChunkIlluminator.cs:162 (relight from chunk `LightPositions`), :935, :1095, BlockAccessorRelaxedBulkUpdate.cs:362, worldgen accessors.
- Static `.LightHsv` is read only for network sync and worldgen height maps, so a per-position override is honoured by everything that matters at runtime.
- `LightPositions` is stored per chunk (index of each block with V>0) and serialized; chunk (re)light resolves the block from chunk data and calls `GetLightHsv(pos)` again. The BE may not be loaded yet then, so the override MUST tolerate a null BE.
- Threading: the call happens off the main thread. Read only a snapshot (a volatile/immutable byte[] field on the BE) and never call `GetBlockEntity` blindly without null checks, never mutate.

## 3. Forcing a local relight without changing the block id
- `IBlockAccessor.ExchangeBlock(sameId, pos)` is not short-circuited (BlockAccessorRelaxed) and with relight=true always enqueues UpdateLighting(old,new,pos). On the server it also sends an ExchangeBlock packet. On the client it is a purely local relight.
- Problem: both old and new hsv are evaluated against the CURRENT state, so brightening works but dimming leaves the old light behind. Fix: call `ba.RemoveBlockLight(oldHsv.Clone(), pos)` BEFORE changing state (server enqueues removal and broadcasts packet 72 so clients remove too), then change state, then `ExchangeBlock(Block.Id,pos)`. This is exactly the vanilla BlockEntityGroundStorage.LightUpdate pattern (`lastLightHsv`, `RemoveBlockLight((byte[])lastLightHsv.Clone(), Pos)`).
- Heavy alternatives: `IWorldManagerAPI.FullRelight(min,max)` (cuboid, resends columns) and `RedrawNeighbouringChunk` (visual only, no relight). Not recommended per-interaction.

## 4. Client
Client has its own illuminator. Server packets: ExchangeBlock (re-applied with relight) and RemoveBlockLight. Race: the ExchangeBlock packet can be processed before the BE tree-attribute update, so the client relight reads stale BE state. Mitigation: in `BlockEntityLamp.FromTreeAttributes` (client) update the snapshot, then run the same remove+same-id-exchange locally.

## 5. Prototype (uncompiled sketch; compile in the port)
```csharp
// BlockLamp
public override byte[] GetLightHsv(IBlockAccessor ba, BlockPos pos, ItemStack stack = null)
{
    if (pos != null && ba.GetBlockEntity(pos) is BlockEntityLamp be) return be.LightSnapshot; // immutable byte[3]
    return base.GetLightHsv(ba, pos, stack);
}
// BlockEntityLamp
volatile byte[] lightSnapshot = {0,0,0};
public byte[] LightSnapshot => lightSnapshot;
void RefreshLight()
{
    var old = lightSnapshot;
    var nu = ComputeHsv(); // lit ? {hue,sat,5*wick+1} : {0,0,0}
    if (old.AsSpan().SequenceEqual(nu)) return;
    if (old[2] > 0) Api.World.BlockAccessor.RemoveBlockLight((byte[])old.Clone(), Pos);
    lightSnapshot = nu;
    Api.World.BlockAccessor.ExchangeBlock(Block.Id, Pos); // same id, relight=true
}
```
Call RefreshLight from ToggleLightedState, ChangeWickHeight, UpdateFuel (incl. fuel exhausted), Initialize, and client FromTreeAttributes. With this the block variants (off/low/med/high) are only needed for the mesh, not light. No Harmony patch of the engine is required.

## 6. Headless test recipe
Test ModSystem (server) registering a chat command: place lamp at P, record `ba.GetLightLevel(P.AddCopy(2,0,0), EnumLightLevelType.OnlyBlockLight)`; call lamp `ChangeWickHeight`/toggle; wait ~500 ms (relight thread); re-read at the same offsets; assert brighter/dimmer/zero. Run on a scratch server, not production.

## 7. Not verified
Salty's Immersive Light / Flickering Lights / Lumos DLLs were not inspected. Prototype not compiled or run. Hue/saturation propagation in UpdateLightAt not examined.

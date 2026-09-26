using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using System;
using System.Linq;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace ImmersiveLighting.Lamps;

public class BlockLamp: BlockLiquidContainerBase
{
    protected string ShapesBasePath => "immersivelighting:" + "shapes/block/lamps/";
    
    // Light comes from the block entity's snapshot (see BlockEntityLamp.RefreshLight). The relight thread calls this
    // for both the old and new block, so it only reads the immutable snapshot and tolerates a missing entity.
    public override byte[] GetLightHsv(IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        if (blockAccessor != null && pos != null)
        {
            var snapshot = (blockAccessor.GetBlockEntity(pos) as BlockEntityLamp)?.LightSnapshot;
            if (snapshot != null) return snapshot;
        }
        return base.GetLightHsv(blockAccessor, pos, stack);
    }

    public override int GetContainerSlotId(BlockPos pos) => 0;

    public override int GetContainerSlotId(ItemStack containerStack) => 0;

    // No per-lamp state lives here: this Block object is shared by every placed lamp of the same variant. Fuel, lit
    // and wick height are read from the BlockEntityLamp at the position in question.
    static BlockEntityLamp LampAt(IWorldAccessor world, BlockPos pos) =>
        pos == null ? null : world?.BlockAccessor.GetBlockEntity(pos) as BlockEntityLamp;

    #region BlockInfo 
    
    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        return base.GetPlacedBlockInteractionHelp(world, selection, forPlayer).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-turn-up"),
                MouseButton = EnumMouseButton.Left,
                HotKeyCode = "shift",
                ShouldApply = ((wi, blockSelection, entitySelection) => (LampAt(world, blockSelection?.Position)?.WickHeight ?? 0) < 3)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-turn-down"),
                MouseButton = EnumMouseButton.Right,
                HotKeyCode = "shift",
                ShouldApply = ((wi, blockSelection, entitySelection) => (LampAt(world, blockSelection?.Position)?.WickHeight ?? 0) > 1)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-douse"),
                MouseButton = EnumMouseButton.Right,
                ShouldApply = ((wi, blockSelection, entitySelection) => LampAt(world, blockSelection?.Position)?.Lit == true)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-light"),
                MouseButton = EnumMouseButton.Right,
                // when the server requires an ignition source, show what can light it (firestarter, lit torch, ...)
                Itemstacks = LampSettings.RequireIgnition(world) ? BlockBehaviorCanIgnite.CanIgniteStacks(api, true)?.ToArray() : null,
                ShouldApply = ((wi, blockSelection, entitySelection) => LampAt(world, blockSelection?.Position)?.Lit != true)
            });
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (api.Side != EnumAppSide.Server) return true;
        BlockEntity blockEntity = world.BlockAccessor.GetBlockEntity(blockSel.Position);
        var handled = blockEntity is BlockEntityLamp && ((BlockEntityLamp)blockEntity).OnPlayerInteract(byPlayer);
        return  handled || base.OnBlockInteractStart(world, byPlayer, blockSel);
    }

    public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
    {
        var info = new StringBuilder();
        var lamp = LampAt(world, pos);
        if (lamp?.Filled == true)
        {
            info.AppendLine(Lang.Get(lamp.HasFuel ? "immersivelighting:lamp-filled-fuel" : "immersivelighting:lamp-filled") + " " + lamp.RemainingFuel + "L");
            lamp.AppendInfo(info);
        }
        return info.ToString();
    }

    #endregion
    
    
    #region Mesh generation
    

        public MeshData GenMesh(ItemStack liquidContentStack, BlockPos forBlockPos = null, bool lit = false)
        {
            ICoreClientAPI capi = api as ICoreClientAPI;

            
            Shape shape = Vintagestory.API.Common.Shape.TryGet(capi, ShapesBasePath + "lamp" + (lit ? "-lit" : "") +  ".json");
            
            MeshData barrelMesh;
            capi.Tesselator.TesselateShape(this, shape, out barrelMesh);

            var containerProps = liquidContentStack?.ItemAttributes?["waterTightContainerProps"];

            MeshData contentMesh =
                    getContentMeshLiquids(liquidContentStack, forBlockPos, containerProps) ??
                    getContentMesh(liquidContentStack, forBlockPos, ShapesBasePath + "contents.json")
                ;

            if (contentMesh != null)
            {
                barrelMesh.AddMeshData(contentMesh);
            }

            if (forBlockPos != null)
            {
                // Water flags
                barrelMesh.CustomInts = new CustomMeshDataPartInt(barrelMesh.FlagsCount);
                barrelMesh.CustomInts.Values.Fill(0x4000000); // light foam only
                barrelMesh.CustomInts.Count = barrelMesh.FlagsCount;

                barrelMesh.CustomFloats = new CustomMeshDataPartFloat(barrelMesh.FlagsCount * 2);
                barrelMesh.CustomFloats.Count = barrelMesh.FlagsCount * 2;
            }

            return barrelMesh;
        }

        private MeshData getContentMeshLiquids(ItemStack liquidContentStack, BlockPos forBlockPos, JsonObject containerProps)
        {
            bool isliquid = containerProps?.Exists == true;
            if (liquidContentStack != null && isliquid)
            {
                var shapefilename = "liquidcontents.json";

                return getContentMesh(liquidContentStack, forBlockPos, ShapesBasePath + shapefilename);
            }

            return null;
        }

        protected MeshData getContentMesh(ItemStack stack, BlockPos forBlockPos, string shapefilepath)
        {
            ICoreClientAPI capi = api as ICoreClientAPI;

            WaterTightContainableProps props = GetContainableProps(stack);
            ITexPositionSource contentSource;
            float fillHeight;

            if (props != null)
            {
                if (props.Texture == null) return null;

                contentSource = new ContainerTextureSource(capi, stack, props.Texture);
                fillHeight = GameMath.Min(1f, stack.StackSize / props.ItemsPerLitre / Math.Max(1, props.MaxStackSize)) * 15f / 16f;
            }
            else
            {
                contentSource = getContentTexture(capi, stack, out fillHeight);
            }


            if (stack != null && contentSource != null)
            {
                Shape shape = Vintagestory.API.Common.Shape.TryGet(capi, shapefilepath);
                if (shape == null)
                {
                    api.Logger.Warning(string.Format("Lamp block '{0}': Content shape {1} not found. Will try to default to another one.", Code, shapefilepath));
                    return null;
                }
                MeshData contentMesh;
                capi.Tesselator.TesselateShape("Lamp", shape, out contentMesh, contentSource, new Vec3f(Shape.rotateX, Shape.rotateY, Shape.rotateZ), props?.GlowLevel ?? 0);

                // contentMesh.Translate(0, fillHeight, 0);
                contentMesh.Scale(new Vec3f(0,0,0), 1,fillHeight, 1);

                if (props?.ClimateColorMap != null)
                {
                    int col = capi.World.ApplyColorMapOnRgba(props.ClimateColorMap, null, ColorUtil.WhiteArgb, 196, 128, false);
                    if (forBlockPos != null)
                    {
                        col = capi.World.ApplyColorMapOnRgba(props.ClimateColorMap, null, ColorUtil.WhiteArgb, forBlockPos.X, forBlockPos.Y, forBlockPos.Z, false);
                    }

                    byte[] rgba = ColorUtil.ToBGRABytes(col);

                    for (int i = 0; i < contentMesh.Rgba.Length; i++)
                    {
                        contentMesh.Rgba[i] = (byte)((contentMesh.Rgba[i] * rgba[i % 4]) / 255);
                    }
                }


                return contentMesh;
            }

            return null;
        }


        public static ITexPositionSource getContentTexture(ICoreClientAPI capi, ItemStack stack, out float fillHeight)
        {
            ITexPositionSource contentSource = null;
            fillHeight = 0;

            JsonObject obj = stack?.ItemAttributes?["inContainerTexture"];
            if (obj != null && obj.Exists)
            {
                contentSource = new ContainerTextureSource(capi, stack, obj.AsObject<CompositeTexture>());
                fillHeight = GameMath.Min(12 / 16f, 0.7f * stack.StackSize / stack.Collectible.MaxStackSize);
            }
            else
            {
                if (stack?.Block != null && (stack.Block.DrawType == EnumDrawType.Cube || stack.Block.Shape.Base.Path.Contains("basic/cube")) && capi.BlockTextureAtlas.GetPosition(stack.Block, "up", true) != null)
                {
                    contentSource = new BlockTopTextureSource(capi, stack.Block);
                    fillHeight = GameMath.Min(12 / 16f, 0.7f * stack.StackSize / stack.Collectible.MaxStackSize);
                }
                else if (stack != null)
                {

                    if (stack.Class == EnumItemClass.Block)
                    {
                        if (stack.Block.Textures.Count > 1) return null;

                        contentSource = new ContainerTextureSource(capi, stack, stack.Block.Textures.FirstOrDefault().Value);
                    }
                    else
                    {
                        if (stack.Item.Textures.Count > 1) return null;

                        contentSource = new ContainerTextureSource(capi, stack, stack.Item.FirstTexture);
                    }


                    fillHeight = GameMath.Min(12 / 16f, 0.7f * stack.StackSize / stack.Collectible.MaxStackSize);
                }
            }

            return contentSource;
        }

        #endregion
}
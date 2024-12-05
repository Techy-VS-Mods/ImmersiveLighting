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
    
    public override int GetContainerSlotId(BlockPos pos) => 0;

    public override int GetContainerSlotId(ItemStack containerStack) => 0;

    public bool HasFuel = false;
    public bool Lit = false;
    public bool Filled = false;
    public double RemainingFuel = 0;
    public int WickHeight = 0;

    #region BlockInfo 
    
    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        return base.GetPlacedBlockInteractionHelp(world, selection, forPlayer).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-turn-up"),
                MouseButton = EnumMouseButton.Left,
                HotKeyCode = "shift",
                ShouldApply = ((wi, blockSelection, entitySelection) => WickHeight < 3)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-turn-down"),
                MouseButton = EnumMouseButton.Right,
                HotKeyCode = "shift",
                ShouldApply = ((wi, blockSelection, entitySelection) => WickHeight > 1)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-douse"),
                MouseButton = EnumMouseButton.Right,
                ShouldApply = ((wi, blockSelection, entitySelection) => Lit)
            }).Append(
            new WorldInteraction()
            {
                ActionLangCode = Lang.Get("immersivelighting:lamp-light"),
                MouseButton = EnumMouseButton.Right,
                ShouldApply = ((wi, blockSelection, entitySelection) => !Lit)
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
        if (Filled)
        {
            info.AppendLine(Lang.Get(HasFuel ? "immersivelighting:lamp-filled-fuel" : "immersivelighting:lamp-filled") + " " + RemainingFuel + "L");
        }

        info.AppendLine();
        info.AppendLine();
        info.AppendLine();
        info.AppendLine();
        info.AppendLine("--- DEBUG ---");
        info.AppendLine("Blockid " + BlockId);
        info.AppendLine("hasfuel " + HasFuel);
        info.AppendLine("lit " + Lit);
        info.AppendLine("filled " + Filled);
        info.AppendLine("remianingFuel " + RemainingFuel);
        info.AppendLine("wickheigh " + WickHeight);
        info.AppendLine("lightHSV " + LightHsv[0] + " " + LightHsv[1] + " " + LightHsv[2]);
        info.AppendLine("Entity uniqueId" + ((BlockEntityLamp)world.BlockAccessor.GetBlockEntity(pos))?.UniqueId);
        return info.ToString();
    }

    #endregion

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        world.BlockAccessor.RemoveBlockLight(LightHsv, pos);
        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
    }

    #region Mesh generation
    
        
        public MeshData GenMesh(ItemStack liquidContentStack, BlockPos forBlockPos = null)
        {
            ICoreClientAPI capi = api as ICoreClientAPI;
        
            
            Shape shape = Vintagestory.API.Common.Shape.TryGet(capi, ShapesBasePath + "lamp" + ( Lit ? "-lit" : "") +  ".json");
            
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
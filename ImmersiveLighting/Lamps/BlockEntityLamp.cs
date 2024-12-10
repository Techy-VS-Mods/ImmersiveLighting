using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using ImmersiveLighting.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.API.MathTools;


namespace ImmersiveLighting.Lamps;

public class BlockEntityLamp : BlockEntityLiquidContainer
{
    private int CapacityLitres { get; set; } = 1;

    public override string InventoryClassName => "lamp";

    private BlockLamp _ownBlock;
    private MeshData _currentMesh;
    private bool _hasFuel;
    private bool _lit = true; // temp
    private int _wickHeight = 1;
    private bool _interactCooldown = true;
    private float _fuelTimer = 0;
    private float _fuelMultiplyer = 1;
    private double _remainingFuel = 0;
    private bool _meshChanged = true;
    private bool _lightChanged = true;
    private const int MAX_WICK = 3;
    public string UniqueId;
    private byte[] currentLightHsv = { 0, 0, 0 };
    
    public BlockEntityLamp()
    {
        inventory = new InventoryGeneric(1, null, null, 
            (id, self) => new ItemSlotLiquidOnly(self, 1f)
            );
        inventory.SlotModified += Inventory_SlotModified;
        inventory.BaseWeight = 1;
    }
    
    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);

        UniqueId = Pos.ToString();
        _ownBlock = Block as BlockLamp;

        RegisterGameTickListener(OnGameTick, 500);
        ImmersiveLightingModSystem.RegisterEntity(UniqueId);
        
        // if (_ownBlock?.Attributes?["capacityLitres"].Exists == true)
        // {
        //     CapacityLitres = _ownBlock.Attributes["capacityLitres"].AsInt(50);
        //     ((ItemSlotLiquidOnly)inventory[0]).CapacityLitres = CapacityLitres;
        // }
        // if (_ownBlock?.Attributes?["filled"].Exists == true)
        // {
        //     _ownBlock.Filled =  _ownBlock.Attributes["filled"].AsBool(false);
        // }
        // if (_ownBlock?.Attributes?["lit"].Exists == true)
        // {
        //     _ownBlock.Lit = _lit = _ownBlock.Attributes["lit"].AsBool(false);
        // }
        // if (_ownBlock?.Attributes?["remainingFuel"].Exists == true)
        // {
        //     _ownBlock.RemainingFuel = _remainingFuel = _ownBlock.Attributes["remainingFuel"].AsDouble(0.0d);
        // }
        // if (_ownBlock?.Attributes?["wickHeight"].Exists == true)
        // {
        //     _ownBlock.WickHeight = _wickHeight = _ownBlock.Attributes["wickHeight"].AsInt(1);
        // }
        
        if (Api?.Side == EnumAppSide.Client)
        {
            _currentMesh = GenMesh();
        }
        
        
        UpdateFuel(0, true);
    }

    protected override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
    {
        if (atBlockFace == BlockFacing.UP) return inventory[0];
        return null;
    }
    
    public bool OnPlayerInteract(IPlayer player)
    {
        if (!_interactCooldown && Api.Side == EnumAppSide.Server)
        {
            if (player.Entity.Controls.ShiftKey) return _interactCooldown = ChangeWickHeight(WickMotion.Up);
            if (player.Entity.Controls.CtrlKey) return _interactCooldown = ChangeWickHeight(WickMotion.Down);
            if (player.Entity.RightHandItemSlot.Empty) return _interactCooldown = ToggleLightedState();
            return false;
        }
        return false;
    }

    private bool ToggleLightedState()
    {
        _lit = !_lit && _hasFuel;
        _lightChanged = true;
        MarkDirty(true);
        return true;
    }
    
    public enum WickMotion
    {
        Up = 1, 
        Down = -1
    }
    /// <summary>
    /// Adjusts Wick Position
    /// This should only be called server side
    /// </summary>
    /// <param name="player"></param>
    /// <param name="motion"></param>
    /// <returns></returns>
    public bool ChangeWickHeight(WickMotion motion)
    {
        var oldHeight = _wickHeight;
        _wickHeight += (int)motion;
        if (_wickHeight is > MAX_WICK or <= 0)
        {
            _wickHeight = oldHeight;
            Api.Logger.Notification("Wick RESET to " + _wickHeight);
            MarkDirty(true);
            return true;
        }
        Api.Logger.Notification("Wick set to" + _wickHeight);
        _lightChanged = true;
        MarkDirty(true);
        return true;
    }
    
    
    public void OnGameTick(float dt)
    {   
        BlockLantern
        _interactCooldown = false;

        if (Api?.Side == EnumAppSide.Client && _meshChanged) _currentMesh = GenMesh();
        
        if (Api?.Side == EnumAppSide.Client)
        {
            ClientTick(dt);
            return;
        }

        if (Api?.Side != EnumAppSide.Server)
        {
            ServerTick(dt);
            return;
        }
    }

    public void ClientTick(float dt)
    {
        UpdateBlockLight();
    }

    public void ServerTick(float dt)
    {
        if (_lit) UpdateFuel(dt);
        UpdateBlockLight(); // This may cause duplication but is required
    }

    public void UpdateFuel(float dt, bool force = false)
    {
        _fuelTimer += dt;
        if (force || _fuelTimer > 30 * _fuelMultiplyer)
        {
            _fuelTimer = 0;
            if (!inventory[0].Empty)
            {
                _hasFuel = inventory[0].Itemstack.Item.CombustibleProps != null;
                if (_hasFuel && _lit)
                {
                    inventory[0].TakeOut(1 * _wickHeight);
                }
            }
            else
            {
                _hasFuel = false;
                _lit = false;
                _lightChanged = true;
            }
            
            _remainingFuel = CalculateRemainingFuel();
            _meshChanged = true;
            MarkDirty(true);
        }
    }
    public int Index3D(int posX, int posY, int posZ)
    {
        
        return (posY % GlobalConstants.ChunkSize * GlobalConstants.ChunkSize + posZ % GlobalConstants.ChunkSize) * GlobalConstants.ChunkSize + posX % GlobalConstants.ChunkSize;
    }

    public override void OnBlockBroken(IPlayer byPlayer = null)
    {
        var ba = Api.World.GetBlockAccessor(true,true, true);
        ba.RemoveBlockLight(Block.LightHsv, Pos);
        ba.Commit();
        base.OnBlockBroken(byPlayer);
    }

    public byte[] GetLightHsv()
    {
        var ba = Api.World.GetBlockAccessor(true,true, true);
        if (currentLightHsv is not null && ba is not null)
        {
            // ba.RemoveBlockLight(currentLightHsv, Pos);
            ba.MarkBlockDirty(Pos); 
            ba.Commit();    
        }
        return UpdateLightHsv(); //CalculatelightHSVSafe(UpdateLightHsv());
    }

    public void UpdateBlockLight()
    {
        
        // Todo instead of using variants try setting lighthsv here before exchanging block
        if (_lightChanged)
        {
            var newBlock = _ownBlock;//(BlockLamp)Api.World.GetBlock(Block.Code);
            
            var ba = Api.World.GetBlockAccessor(true,true, true);
            var chunk = ba.GetChunk(Pos.X / GlobalConstants.ChunkSize, Pos.InternalY / GlobalConstants.ChunkSize, Pos.Z / GlobalConstants.ChunkSize);
            // var pos = new Vec2i(Pos.X / GlobalConstants.ChunkSize, Pos.Z / GlobalConstants.ChunkSize);
            // ba.RemoveBlockLight(Block.LightHsv, Pos);
            var newLightHsv = UpdateLightHsv();
            // newBlock.Lit = _lit;
            // newBlock.HasFuel = _hasFuel;
            // newBlock.Filled = !inventory[0].Empty;
            // newBlock.RemainingFuel = _remainingFuel;
            // newBlock.WickHeight = _wickHeight;
            // newBlock.LightHsv = CalculatelightHSVSafe(newLightHsv);
            // newBlock.LightAbsorption = 1;
            
            
            //rgba works
            // or should I say bgra or argb
            newBlock.ParticleProperties = !_lit
                ? Array.Empty<AdvancedParticleProperties>()
                : new[]
                {
                    new AdvancedParticleProperties()
                    {
                        // Fire Quads
                        HsvaColor = new[]
                        {
                            NatFloat.createUniform(newLightHsv[0],0),
                            NatFloat.createUniform(newLightHsv[1], 0),
                            NatFloat.createUniform(newLightHsv[2], 0),
                            NatFloat.createUniform(225, 0)
                        },
                        OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -16),
                        GravityEffect = NatFloat.Zero,
                        PosOffset = new[]
                        {
                            NatFloat.createUniform(-0.005f, 0.002f),
                            NatFloat.createUniform(-.3f, 0f),
                            NatFloat.createUniform(-0.01f, 0.02f)
                        },
                        Velocity = new[]
                        {
                            NatFloat.createUniform(0f, 0f),
                            NatFloat.createUniform(0.01f, 0.02f),
                            NatFloat.createUniform(0f, 0f)
                        },
                        Quantity = NatFloat.createUniform(1.03f, 0.01f),
                        Size = NatFloat.createUniform(0.03f, 0.03f),
                        LifeLength = NatFloat.createUniform(0.3f, 0.1f),
                        SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, 0.15f),
                        ParticleModel = EnumParticleModel.Quad,
                        VertexFlags = 128,
                        WindAffectednes = 0
                    }
                };
            
            ba.ExchangeBlock(newBlock.BlockId, Pos);
            ba.MarkBlockModified(Pos);
            // ba.MarkBlockModified(Pos);
            // chunk.MarkModified();
            ba.Commit();
            
            //                     int HSVbitflag = (ColorUtil.Rgb2Hsv((float) color1[0], (float) color1[1], (float) color1[2]) | -16777216) >> 8;
            //                      int rgbBitFlag = ColorUtil.Hsv2Rgb((num1 & 65280) S+ ((num1 & (int) byte.MaxValue) << 16) + (num1 >> 16 & (int) byte.MaxValue));
;            _ownBlock = newBlock;
            _meshChanged = true;
            _lightChanged = false;
        }
    }


    public byte[] CalculatelightHSVSafe(byte[] lightHsvUnsafe)
    {
        byte[] lightHsvSafe = {0,0,0};
        lightHsvSafe[0] = (byte)(63 * (lightHsvUnsafe[0] / 255d));
        lightHsvSafe[1] = (byte)(7 * (lightHsvUnsafe[1] / 255d));
        lightHsvSafe[2] = (byte)(31 * (lightHsvUnsafe[2] / 255d));
        Api.Logger.Notification("Calculating lights safe. From " + lightHsvUnsafe[0] + ", " + lightHsvUnsafe[1] + ", " + lightHsvUnsafe[2] + " to " + lightHsvSafe[0] + ", " + lightHsvSafe[1] + ", " + lightHsvSafe[2]);
        return lightHsvSafe;
    }
    
    private byte[] UpdateLightHsv()
    {
        byte[] lightHsv = { 0, 0, 0 };

        if (!inventory[0].Empty)
        {
            if (_lit)
            {
                // var color = ColorCalculators.GetRGBFromCelsius((double)inventory[0].Itemstack.Item.CombustibleProps.BurnTemperature);
                // var hsv = ColorUtil.RgbToHsvInts(color[0], color[1], color[2]).Select(x => (byte)x).ToArray();
                // hsv[2] = (byte)((int)hsv[2] > 255 ? 255 : hsv[2]);
                // //hsv[2] = (byte) (( ((255 / hsv[2]) / MAX_WICK) * _wickHeight) * 100);
                // hsv[2] = (byte) (hsv[2] * (_wickHeight / MAX_WICK));
                var mat = MaterialRepository.MaterialDatabase()[
                    new Random().Next(0, MaterialRepository.MaterialDatabase().Count - 1)];
                lightHsv = FlameColorCalculatorMaterial.GetFlameColor(mat.Name, new CombustionContext(){ HighOxygen = false, LargeFlame = false});
                // lightHsv = FlameColorCalculatorTemp.CalculateFlameHSV(1200, "isopropyl_alcohol", false);
                lightHsv[2] = (byte)Math.Round(lightHsv[2] * ((double)_wickHeight / MAX_WICK));
                //lightHsv = FlameColorCalculator.GetFlameColorFromTemperature(inventory[0].Itemstack.Item.CombustibleProps.BurnTemperature);
                //lightHsv = new byte[] { colortemp, 5, (byte)(5 * _wickHeight + 1) };
            }    
        }

        return currentLightHsv = lightHsv;
    }
    
    
    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        base.ToTreeAttributes(tree);
        var stuff = new StringBuilder().AppendLine("hasFuel " + _hasFuel).AppendLine("lit " + _lit).AppendLine("hasContents " + !inventory[0].Empty).AppendLine("remainingFuel " + _remainingFuel).AppendLine("wickHeight " + _wickHeight).AppendLine("lightChangeds " + _lightChanged);
        tree.SetBool("hasFuel" + UniqueId, _hasFuel);
        tree.SetBool("lit" + UniqueId, _lit);
        tree.SetBool("filled" + UniqueId, !inventory[0].Empty);
        tree.SetDouble("remainingFuel" + UniqueId, _remainingFuel);
        tree.SetInt("wickHeight" + UniqueId, _wickHeight);
        tree.SetBool("lightChanged" + UniqueId, _lightChanged);
    }

    private double CalculateRemainingFuel()
    {
        if (inventory[0].Empty) return 0;
        var liquidProps = inventory[0].Itemstack.ItemAttributes?["waterTightContainerProps"].AsObject<WaterTightContainableProps>() ;
        if (liquidProps != null)
        {
            return Math.Round(inventory[0].Itemstack.StackSize / liquidProps.ItemsPerLitre, 2);
        }

        return 0;
    }
    
    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
    {
        base.FromTreeAttributes(tree, worldForResolving);
        // var stuff = new StringBuilder().AppendLine("hasFuel " + _hasFuel).AppendLine("lit " + _lit).AppendLine("hasContents " + !inventory[0].Empty).AppendLine("remainingFuel " + _remainingFuel).AppendLine("wickHeight " + _wickHeight).AppendLine("lightChangeds " + _lightChanged);
        ((BlockLamp)Block).HasFuel = _hasFuel = tree.GetBool("hasFuel" + UniqueId);
        ((BlockLamp)Block).Lit = _lit = tree.GetBool("lit" + UniqueId);
        ((BlockLamp)Block).Filled = tree.GetBool("filled" + UniqueId);
        ((BlockLamp)Block).RemainingFuel = _remainingFuel = tree.GetDouble("remainingFuel" + UniqueId);
        ((BlockLamp)Block).WickHeight = _wickHeight = tree.GetInt("wickHeight" + UniqueId);
        _lightChanged = tree.GetBool("lightChanged" + UniqueId, true);
        // var stuff2 = new StringBuilder().AppendLine("hasFuel " + _hasFuel).AppendLine("lit " + _lit).AppendLine("hasContents " + !inventory[0].Empty).AppendLine("remainingFuel " + _remainingFuel).AppendLine("wickHeight " + _wickHeight).AppendLine("lightChangeds " + _lightChanged);
        _currentMesh = GenMesh();
        MarkDirty(true);
        
    }

    private void Inventory_SlotModified(int slotId)
    {
        if (slotId == 0)
        {
            var combustProps = inventory[0]?.Itemstack?.Item?.CombustibleProps;
            _hasFuel = combustProps != null;
            _fuelMultiplyer = combustProps?.BurnDuration ?? 1;
            _meshChanged = true;
            UpdateFuel(0, true);
            MarkDirty(true);
        }

    }
    
    internal MeshData GenMesh()
    {
        if (_ownBlock == null) return null;
    
        MeshData mesh = _ownBlock.GenMesh(inventory[0].Itemstack, Pos);
    
        if (mesh.CustomInts != null)
        {
            for (int i = 0; i < mesh.CustomInts.Count; i++)
            {
                // do I need this????
                mesh.CustomInts.Values[i] |= 1 << 27; // Enable weak water wavy
                mesh.CustomInts.Values[i] |= 1 << 26; // Enabled weak foam
            }
        }
    
        return mesh;
    }
    
    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    {
        mesher.AddMeshData(_currentMesh);
        return true;
    }
}
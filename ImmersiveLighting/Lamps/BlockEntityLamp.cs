using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ImmersiveLighting.Lamps;

public enum BlockLampStates
{
    off,
    low,
    med,
    high
}

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
    private bool _filledClient;

    // Per-lamp state. Everything below belongs to THIS placed lamp; the Block object is shared by every lamp of the same
    // variant, so state must never be stored on it (that made every lamp show the fuel level of the last one updated).
    public bool HasFuel => _hasFuel;
    public bool Lit => _lit;
    public bool Filled => Api?.Side == EnumAppSide.Client ? _filledClient : !inventory[0].Empty;
    public double RemainingFuel => _remainingFuel;
    public int WickHeight => _wickHeight;

    // Per-position light, read off the main thread by BlockLamp.GetLightHsv during relight, so it is an immutable
    // array that is swapped atomically. null until the first RefreshLight: a chunk relight that runs before Initialize
    // then falls back to the variant's static light instead of reading a half-initialised entity.
    private volatile byte[] _lightSnapshot;
    public byte[] LightSnapshot => _lightSnapshot;

    // Test hook only: skips RemoveBlockLight to reproduce "dimming leaves the old light behind".
    public static bool DebugNaiveRelight;

    private BlockLampStates CurrentState
    {
        get
        {
            var parsed = BlockLampStates.TryParse(Block.Code.EndVariant(), out BlockLampStates state);
            if (!parsed) Api.Logger.Warning("Failed to parse BlockEntityLamp current state. " + Block.Code.EndVariant());
            return parsed ? state : BlockLampStates.off;
        }
    }
    private BlockLampStates NewState { get {
        var calculatedWickHeight = _wickHeight;
        if (_remainingFuel < 0.15d) calculatedWickHeight = GameMath.Min(2, _wickHeight);
        if (_remainingFuel < 0.5d) calculatedWickHeight = GameMath.Min(1, _wickHeight);
        
        return _lit ? (BlockLampStates)calculatedWickHeight : BlockLampStates.off;
        
    } }

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

        _ownBlock = Block as BlockLamp;

        RegisterGameTickListener(OnGameTick, 500);
        if (api.Side == EnumAppSide.Client) RegisterGameTickListener(SpawnSmoke, 250);
        
        if (_ownBlock?.Attributes?["capacityLitres"].Exists == true)
        {
            CapacityLitres = _ownBlock.Attributes["capacityLitres"].AsInt(50);
            ((ItemSlotLiquidOnly)inventory[0]).CapacityLitres = CapacityLitres;
        }
        if (_ownBlock?.Attributes?["lit"].Exists == true)
        {
            _lit = _ownBlock.Attributes["lit"].AsBool(false);
        }
        if (_ownBlock?.Attributes?["remainingFuel"].Exists == true)
        {
            _remainingFuel = _ownBlock.Attributes["remainingFuel"].AsDouble(0.0d);
        }
        if (_ownBlock?.Attributes?["wickHeight"].Exists == true)
        {
            _wickHeight = _ownBlock.Attributes["wickHeight"].AsInt(1);
        }
        if (Api?.Side == EnumAppSide.Client)
        {
            _currentMesh = GenMesh();
        }
        
        
        UpdateFuel(0, true);
        UpdateBlock();
    }

    protected override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
    {
        if (atBlockFace == BlockFacing.UP) return inventory[0];
        return null;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <returns>True if interaction handled otherwise false</returns>
    public bool OnPlayerInteract(IPlayer player)
    {
        if (!_interactCooldown && Api.Side == EnumAppSide.Server)
        {
            if (player.Entity.Controls.ShiftKey) return _interactCooldown = ChangeWickHeight(WickMotion.Up);
            if (player.Entity.Controls.CtrlKey) return _interactCooldown = ChangeWickHeight(WickMotion.Down);
            if (player.Entity.RightHandItemSlot.Empty) return ToggleLightedState();
            return false;
        }
        return false;
    }

    private bool ToggleLightedState()
    {
        _lit = !_lit && _hasFuel;
        RefreshLight();
        return true;
    }

    /// <summary>
    /// Light for the lamp's current state, using the same brightness steps as the off/low/med/high variants
    /// (V 0/5/10/20). The variants now only drive the mesh; light comes from this snapshot via BlockLamp.GetLightHsv.
    /// </summary>
    private byte[] ComputeHsv()
    {
        int wick = (int)NewState;
        if (wick <= 0) return new byte[] { 0, 0, 0 };

        // Colour comes from what is burning; brightness is the wick's base level scaled by how luminous the flame is
        // (soot glows: clean spirit flames are dim, oils bright) and reduced by the smoke that escapes instead of glowing.
        var fuel = FuelProfile.For(inventory[0].Itemstack);
        float scale = _ownBlock?.Attributes?["brightnessScale"].AsFloat(1f) ?? 1f;
        float v = BaseBrightness[wick - 1] * fuel.Luminosity * scale * (1f - 0.5f * fuel.EffectiveSmoke(wick));
        return new byte[] { fuel.Hue, fuel.Sat, (byte)GameMath.Clamp((int)Math.Round(v), 1, 32) };
    }

    private static readonly int[] BaseBrightness = { 5, 10, 20 };

    private SimpleParticleProperties _smokeProps;

    /// <summary>Client only: soot rising from the flame. Purely visual; density follows the fuel's smoke value and the wick height.</summary>
    private void SpawnSmoke(float dt)
    {
        if (Api?.Side != EnumAppSide.Client || !_lit || inventory[0].Empty) return;
        float scale = ImmersiveLightingConfig.Current.SmokeScale;
        int wick = (int)NewState;
        if (scale <= 0f || wick <= 0) return;

        float smoke = FuelProfile.For(inventory[0].Itemstack).EffectiveSmoke(wick) * scale;
        if (smoke < 0.02f) return;

        _smokeProps ??= new SimpleParticleProperties(0, 1, ColorUtil.ToRgba(140, 70, 70, 70), new Vec3d(), new Vec3d(),
            new Vec3f(-0.01f, 0.10f, -0.01f), new Vec3f(0.01f, 0.16f, 0.01f), 2.2f, -0.02f, 0.10f, 0.22f, EnumParticleModel.Quad)
        {
            OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, -140),
            SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, 0.5f),
            WindAffected = true,
            ParticleModel = EnumParticleModel.Quad
        };
        _smokeProps.MinPos = new Vec3d(Pos.X + 0.42, Pos.Y + 0.85, Pos.Z + 0.42);
        _smokeProps.AddPos = new Vec3d(0.16, 0.05, 0.16);
        _smokeProps.MinQuantity = 0;
        _smokeProps.AddQuantity = Math.Max(0.4f, smoke * 3f);
        _smokeProps.Color = ColorUtil.ToRgba((int)GameMath.Clamp(70 + 170 * smoke, 70, 220), 70, 70, 70);
        Api.World.SpawnParticles(_smokeProps);
    }

    /// <summary>
    /// Forces a light recalculation when the lamp's light changes without the block id changing. The engine only
    /// relights from queued block-change tasks, and an exchange reads the old and new light from the CURRENT state,
    /// so dimming would leave the old light behind: the old contribution is removed first, then the block is
    /// re-exchanged to enqueue the relight (same pattern as vanilla ground storage). Safe on both sides.
    /// </summary>
    public void RefreshLight()
    {
        if (Api == null || Pos == null) return;
        var previous = _lightSnapshot ?? Block?.LightHsv ?? new byte[] { 0, 0, 0 };
        var next = ComputeHsv();
        if (_lightSnapshot != null && previous.AsSpan().SequenceEqual(next)) return;

        var ba = Api.World.BlockAccessor;
        if (!DebugNaiveRelight && previous.Length >= 3 && previous[2] > 0)
        {
            ba.RemoveBlockLight((byte[])previous.Clone(), Pos);
        }
        _lightSnapshot = next;
        ba.ExchangeBlock(ba.GetBlock(Pos).Id, Pos);
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
        if (_wickHeight is > 3 or <= 0)
        {
            _wickHeight = oldHeight;
            Api.Logger.Notification("Wick RESET to " + _wickHeight);
            MarkDirty(true);
            return true;
        }
        Api.Logger.Notification("Wick set to" + _wickHeight);
        RefreshLight();
        MarkDirty(true);
        return true;
    }
    
    
    public void OnGameTick(float dt)
    {   
        _interactCooldown = false;

        if (Api?.Side == EnumAppSide.Client && _meshChanged) _currentMesh = GenMesh();

        if (Api?.Side != EnumAppSide.Server) return;
        UpdateBlock();
        UpdateFuel(dt);
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
            }
            
            _remainingFuel = CalculateRemainingFuel();
            RefreshLight();
            _meshChanged = true;
            MarkDirty(true);
        }
    }

    public void UpdateBlock()
    {
        
        // Todo instead of using variants try setting lighthsv here before exchanging block
        if (NewState != CurrentState)
        {
            var  newBlock = (BlockLamp) Api.World.GetBlock(Block.CodeWithParts(NewState.ToString()));
            
            // var  newBlock = (BlockLamp) Api.World.GetBlock(Block.CodeWithParts("off"));
            // newBlock.LightHsv = GetLightHsv();
            Api.World.BlockAccessor.ExchangeBlock(newBlock.BlockId, Pos);
            _ownBlock = newBlock;
            _meshChanged = false;
        }
    }
    
    // private byte[] GetLightHsv()
    // {
    //     byte[] lightHsv = { 0, 0, 0 };
    //
    //     // ReSharper disable once PossibleLossOfFraction
    //     var colortemp = (byte)GameMath.Clamp(Math.Round((double)inventory[0].Itemstack.Item.CombustibleProps.BurnTemperature / 100), 3, 11);
    //     // 4-10
    //     
    //     if (lit)
    //     {
    //         lightHsv = new byte[] { colortemp, 5, (byte)(5 * wickHeight + 1) };
    //     }
    //     
    //     return lightHsv;
    // }
    
    
    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        base.ToTreeAttributes(tree);
        if (Api?.Side == EnumAppSide.Server)
        {
             tree.SetBool("hasFuel", _hasFuel);
             tree.SetBool("lit", _lit);
             tree.SetBool("filled", !inventory[0].Empty);
             tree.SetDouble("remainingFuel", _remainingFuel);
             tree.SetDouble("wickHeight", _wickHeight);
        }
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
        if (Api?.Side == EnumAppSide.Client)
        {
            _hasFuel = tree.GetBool("hasFuel");
            _lit = tree.GetBool("lit");
            _filledClient = tree.GetBool("filled");
            _remainingFuel = tree.GetDouble("remainingFuel");
            _wickHeight = (int)tree.GetDouble("wickHeight", _wickHeight);

            // The server's ExchangeBlock packet can be processed before this attribute update, so the client relight
            // would read stale state. Refresh from the freshly received state here.
            RefreshLight();

            _currentMesh = GenMesh();
            MarkDirty(true);
        }
        
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

        MeshData mesh = _ownBlock.GenMesh(inventory[0].Itemstack, Pos, _lit);

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
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
        
        if (_ownBlock?.Attributes?["capacityLitres"].Exists == true)
        {
            CapacityLitres = _ownBlock.Attributes["capacityLitres"].AsInt(50);
            ((ItemSlotLiquidOnly)inventory[0]).CapacityLitres = CapacityLitres;
        }
        if (_ownBlock?.Attributes?["filled"].Exists == true)
        {
            _ownBlock.Filled =  _ownBlock.Attributes["filled"].AsBool(false);
        }
        if (_ownBlock?.Attributes?["lit"].Exists == true)
        {
            _ownBlock.Lit = _lit = _ownBlock.Attributes["lit"].AsBool(false);
        }
        if (_ownBlock?.Attributes?["remainingFuel"].Exists == true)
        {
            _ownBlock.RemainingFuel = _remainingFuel = _ownBlock.Attributes["remainingFuel"].AsDouble(0.0d);
        }
        if (_ownBlock?.Attributes?["wickHeight"].Exists == true)
        {
            _ownBlock.WickHeight = _wickHeight = _ownBlock.Attributes["wickHeight"].AsInt(1);
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
        if (_wickHeight is > 3 or <= 0)
        {
            _wickHeight = oldHeight;
            Api.Logger.Notification("Wick RESET to " + _wickHeight);
            MarkDirty(true);
            return true;
        }
        Api.Logger.Notification("Wick set to" + _wickHeight);
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
            newBlock.Lit = _lit;
            newBlock.HasFuel = _hasFuel;
            newBlock.Filled = !inventory[0].Empty;
            newBlock.RemainingFuel = _remainingFuel;
            newBlock.WickHeight = _wickHeight;
            
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
            ((BlockLamp)Block).HasFuel = _hasFuel = tree.GetBool("hasFuel");
            ((BlockLamp)Block).Lit = _lit = tree.GetBool("lit");
            ((BlockLamp)Block).Filled = tree.GetBool("filled");
            ((BlockLamp)Block).RemainingFuel = _remainingFuel = tree.GetDouble("remainingFuel");
            
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
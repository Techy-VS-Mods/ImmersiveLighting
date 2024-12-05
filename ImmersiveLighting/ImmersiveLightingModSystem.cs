using System;
using System.Collections.Generic;
using ImmersiveLighting.Lamps;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace ImmersiveLighting;

public class ImmersiveLightingModSystem : ModSystem
{
    public static ICoreServerAPI CoreServerApi { get; private set; }
    public static List<string> EntityGuids = new List<string>();
    
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {;
        api.RegisterBlockEntityClass("BlockEntityLamp", typeof(BlockEntityLamp));
        api.RegisterBlockClass("BlockLamp", typeof(BlockLamp));
    }

    public static void RegisterEntity(string guid)
    {
        EntityGuids.Add(guid);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        CoreServerApi = api;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
    }
}
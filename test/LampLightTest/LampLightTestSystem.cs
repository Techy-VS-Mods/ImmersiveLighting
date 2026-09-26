using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ImmersiveLighting.Lamps;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LampLightTest
{
    // Server-side harness. Runs one scripted scenario after the world is up, writes light readings to
    // lamptest-results.txt next to the save, then shuts the server down. Set env LAMPTEST_OUT to override the path.
    public class LampLightTestSystem : ModSystem
    {
        ICoreServerAPI sapi;
        readonly StringBuilder log = new StringBuilder();
        readonly List<(string name, int delayMs, Action act)> steps = new List<(string, int, Action)>();
        int stepIdx;
        BlockPos P, Q, R, T1;
        BlockEntityLamp lamp, lamp2;
        BlockPos S;
        bool instancingOk = true;

        public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;
            api.Event.ServerRunPhase(EnumServerRunPhase.RunGame, () => api.Event.RegisterCallback(dt => Begin(), 6000));
        }

        int Light(BlockPos p) => sapi.World.BlockAccessor.GetLightLevel(p, EnumLightLevelType.OnlyBlockLight);
        string Snap(string label)
        {
            var ba = sapi.World.BlockAccessor;
            var s = string.Format("{0,-46} P={1,2} Q(+2)={2,2} R(+4)={3,2} | block@P={4}", label, Light(P), Light(Q), Light(R), ba.GetBlock(P).Code);
            if (lamp != null)
            {
                var t = lamp.GetType();
                object f(string n) => t.GetField(n, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(lamp);
                s += string.Format(" | lit={0} hasFuel={1} wick={2} remaining={3}", f("_lit"), f("_hasFuel"), f("_wickHeight"), f("_remainingFuel"));
            }
            log.AppendLine(s);
            sapi.Logger.Notification("[LampTest] " + s);
            return s;
        }

        void Begin()
        {
            var ba = sapi.World.BlockAccessor;
            var sp = sapi.World.DefaultSpawnPosition.AsBlockPos;
            int y = ba.GetTerrainMapheightAt(sp) + 2;
            P = new BlockPos(sp.X + 6, y, sp.Z + 6); Q = P.AddCopy(2, 0, 0); R = P.AddCopy(4, 0, 0); T1 = P.AddCopy(0, 0, 8);
            log.AppendLine("spawn=" + sp + " P=" + P + " chunkLoaded=" + (ba.GetChunkAtBlockPos(P) != null));
            var lampOff = sapi.World.GetBlock(new AssetLocation("immersivelighting:lamp-off"));
            var torchLit = sapi.World.GetBlock(new AssetLocation("game:torch-basic-lit-up"));
            var torchOut = sapi.World.GetBlock(new AssetLocation("game:torch-basic-extinct-up"));
            log.AppendLine("blocks resolved: lamp-off=" + (lampOff != null) + " torchLit=" + (torchLit != null) + " torchOut=" + (torchOut != null));
            if (lampOff == null || torchLit == null || torchOut == null) { Finish("FATAL: blocks missing"); return; }
            var fuel = sapi.World.GetItem(new AssetLocation("game:alcoholportion"));
            log.AppendLine("fuel item resolved: " + (fuel != null));

            // ---- control: does ExchangeBlock relight, compared to SetBlock? (vanilla torch, P + 8 on Z)
            steps.Add(("C1 place lit torch via SetBlock", 1500, () => ba.SetBlock(torchLit.BlockId, T1)));
            steps.Add(("C1 read", 0, () => LogAt("C1 torch lit (SetBlock)", T1)));
            steps.Add(("C2 ExchangeBlock lit->extinct", 1500, () => ba.ExchangeBlock(torchOut.BlockId, T1)));
            steps.Add(("C2 read", 0, () => LogAt("C2 after ExchangeBlock to extinct", T1)));
            steps.Add(("C3 ExchangeBlock extinct->lit", 1500, () => ba.ExchangeBlock(torchLit.BlockId, T1)));
            steps.Add(("C3 read", 0, () => LogAt("C3 after ExchangeBlock back to lit", T1)));
            steps.Add(("C4 SetBlock extinct", 1500, () => ba.SetBlock(torchOut.BlockId, T1)));
            steps.Add(("C4 read", 0, () => LogAt("C4 after SetBlock to extinct", T1)));

            // ---- the lamp scenario, driven the way the mod drives itself
            steps.Add(("L0 place lamp-off", 1500, () => { ba.SetBlock(lampOff.BlockId, P); lamp = ba.GetBlockEntity(P) as BlockEntityLamp; log.AppendLine("lamp BE present=" + (lamp != null)); }));
            steps.Add(("L0 read", 0, () => Snap("L0 empty lamp placed")));
            steps.Add(("L1 fill with fuel", 1500, () => { lamp.Inventory[0].Itemstack = new ItemStack(fuel, 100); lamp.Inventory[0].MarkDirty(); }));
            steps.Add(("L1 read", 0, () => Snap("L1 fuel added, not lit")));
            steps.Add(("L2 light lamp", 2500, () => Call("ToggleLightedState")));
            steps.Add(("L2 read", 0, () => Snap("L2 lit (wick 1)")));
            steps.Add(("L3 wick up", 2500, () => lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Up)));
            steps.Add(("L3 read", 0, () => Snap("L3 wick 2")));
            steps.Add(("L4 wick up", 2500, () => lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Up)));
            steps.Add(("L4 read", 0, () => Snap("L4 wick 3")));
            steps.Add(("L5 wick down", 2500, () => lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Down)));
            steps.Add(("L5 read", 0, () => Snap("L5 wick back to 2")));
            steps.Add(("L6 fuel runs out", 3500, () => { lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty(); }));
            steps.Add(("L6 read", 0, () => Snap("L6 fuel removed (should go dark)")));
            steps.Add(("L6b wait more", 5000, () => { }));
            steps.Add(("L6b read", 0, () => Snap("L6b 5s later")));
            // ---- regression: two placed lamps must each report their OWN fuel (BlockLamp is shared by every lamp of a variant)
            steps.Add(("S1 place second lamp, different fuel", 1500, () =>
            {
                S = P.AddCopy(0, 0, -5);
                ba.SetBlock(lampOff.BlockId, S);
                lamp2 = ba.GetBlockEntity(S) as BlockEntityLamp;
                lamp.Inventory[0].Itemstack = new ItemStack(fuel, 100); lamp.Inventory[0].MarkDirty();   // 1.0 L
                lamp2.Inventory[0].Itemstack = new ItemStack(fuel, 30); lamp2.Inventory[0].MarkDirty();  // 0.3 L
            }));
            steps.Add(("S1 read", 1500, () => CheckInstancing()));
            steps.Add(("S2 change lamp B only", 1500, () => { lamp2.Inventory[0].Itemstack = new ItemStack(fuel, 70); lamp2.Inventory[0].MarkDirty(); }));
            steps.Add(("S2 read", 1500, () => CheckInstancing()));
            steps.Add(("done", 0, () => Finish(instancingOk ? "OK" : "FAIL: lamps share state (instancing bug)")));
            Next();
        }

        void CheckInstancing()
        {
            var ba = sapi.World.BlockAccessor;
            string InfoAt(BlockPos p) => ((BlockLamp)ba.GetBlock(p)).GetPlacedBlockInfo(sapi.World, p, null).Trim();
            string a = InfoAt(P), b = InfoAt(S);
            bool sameBlockObject = ReferenceEquals(ba.GetBlock(P), ba.GetBlock(S));
            // the state must not live on the shared Block at all
            var leaked = new List<string>();
            foreach (var n in new[] { "HasFuel", "Lit", "Filled", "RemainingFuel", "WickHeight" })
                if (typeof(BlockLamp).GetField(n, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null) leaked.Add(n);
            bool ok = a != b && a.Length > 0 && b.Length > 0 && a.Contains(lamp.RemainingFuel + "L") && b.Contains(lamp2.RemainingFuel + "L") && leaked.Count == 0;
            var msg = string.Format("INSTANCING sameBlockObject={0} | lamp A ({1}L): \"{2}\" | lamp B ({3}L): \"{4}\" | state fields on shared BlockLamp: [{5}] => {6}",
                sameBlockObject, lamp.RemainingFuel, a, lamp2.RemainingFuel, b, string.Join(",", leaked), ok ? "PASS" : "FAIL");
            log.AppendLine(msg); sapi.Logger.Notification("[LampTest] " + msg);
            if (!ok) instancingOk = false;
        }

        void LogAt(string label, BlockPos p)
        {
            var ba = sapi.World.BlockAccessor;
            var s = string.Format("{0,-46} at={1,2} +2={2,2} +4={3,2} | block={4}", label, Light(p), Light(p.AddCopy(2, 0, 0)), Light(p.AddCopy(4, 0, 0)), ba.GetBlock(p).Code);
            log.AppendLine(s); sapi.Logger.Notification("[LampTest] " + s);
        }

        void Call(string method)
        {
            var m = lamp.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            m.Invoke(lamp, null);
        }

        void Next()
        {
            if (stepIdx >= steps.Count) return;
            var (name, delay, act) = steps[stepIdx++];
            try { act(); } catch (Exception e) { log.AppendLine("STEP FAILED " + name + ": " + e); }
            sapi.Event.RegisterCallback(dt => Next(), Math.Max(50, delay));
        }

        void Finish(string status)
        {
            log.AppendLine("STATUS: " + status);
            var path = Environment.GetEnvironmentVariable("LAMPTEST_OUT") ?? "/tmp/vs_lighttest/lamptest-results.txt";
            File.WriteAllText(path, log.ToString());
            sapi.Logger.Notification("[LampTest] finished: " + status + " -> " + path);
            sapi.Event.RegisterCallback(dt => sapi.Server.ShutDown(), 1000);
        }
    }
}

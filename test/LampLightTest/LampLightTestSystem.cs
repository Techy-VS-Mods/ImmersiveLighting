using System;
using System.Linq;
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
        bool instancingOk = true, fuelsOk = true, flamesOk = true, featuresOk = true;

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
            // ---- fuel compat: every liquid fuel from a mod that IS installed must carry burn properties and be accepted by the lamp
            steps.Add(("F1 fuel compat", 1500, () => CheckFuels()));
            steps.Add(("F2 flame colour + brightness per fuel", 1500, () => CheckFlames()));
            steps.Add(("I1 ignition, readout and settings", 1500, () => CheckFeatures()));
            steps.Add(("F3 world light with olive oil at wick 3", 1500, () => LightWorldCheck("game:oilportion-olive", ExpectedV(0.95, 0.15, 3))));
            steps.Add(("F3 read", 3500, () => ReadWorldLight()));

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
            steps.Add(("done", 0, () => Finish(!instancingOk ? "FAIL: lamps share state (instancing bug)" : !fuelsOk ? "FAIL: fuel compat" : !flamesOk ? "FAIL: flame profile" : !featuresOk ? "FAIL: ignition/readout/settings" : "OK")));
            Next();
        }

        // A trailing * means "every variant with that prefix"; every variant of every installed fuel must pass.
        static readonly string[] FuelCodes =
        {
            "game:alcoholportion", "game:spiritportion*", "game:oilportion-*",
            "expandedfoods:foodoilportion-*", "expandedfoods:lard", "expandedfoods:strongspiritportion-*", "expandedfoods:potentspiritportion-*",
            "dairyplus:ghee", "bdorchard:oilportion-*", "bdorchard:spiritportion-*", "bdcrop:spiritportion-*"
        };

        IEnumerable<Item> ResolveFuel(string code)
        {
            if (!code.EndsWith("*")) { var one = sapi.World.GetItem(new AssetLocation(code)); return one == null ? new Item[0] : new[] { one }; }
            var prefix = code.TrimEnd('*');
            return sapi.World.Items.Where(i => i?.Code != null && i.Code.ToString().StartsWith(prefix)).ToList();
        }

        void CheckFuels()
        {
            int present = 0, good = 0, absentCodes = 0;
            foreach (var code in FuelCodes)
            {
                var items = ResolveFuel(code).ToList();
                if (items.Count == 0) { absentCodes++; log.AppendLine(string.Format("FUEL {0,-44} absent (mod not installed)", code)); continue; }
                foreach (var item in items)
                {
                    present++;
                    var cp = item.CombustibleProps;
                    lamp.Inventory[0].Itemstack = new ItemStack(item, 10); lamp.Inventory[0].MarkDirty();
                    bool accepted = lamp.Inventory[0].Itemstack != null;
                    var prof = item.Attributes?["immersivelighting"];
                    bool hasProfile = prof != null && prof.Exists && prof["flameHue"].Exists && prof["luminosity"].Exists && prof["smoke"].Exists;
                    bool ok = cp != null && accepted && lamp.HasFuel && hasProfile;
                    if (ok) good++; else fuelsOk = false;
                    var line = string.Format("FUEL {0,-48} temp={1,5} dur={2,4} (~{3,3:0} min/L at wick 1) accepted={4} hasFuel={5} profile={6} => {7}",
                        item.Code, cp?.BurnTemperature, cp?.BurnDuration, (cp?.BurnDuration ?? 0) * 50, accepted, lamp.HasFuel, hasProfile, ok ? "PASS" : "FAIL");
                    log.AppendLine(line); sapi.Logger.Notification("[LampTest] " + line);
                    lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
                }
            }
            log.AppendLine(string.Format("FUELCHECK {0} of {1} installed fuel variants OK ({2} fuel families absent)", good, present, absentCodes));
        }

        // fuel, expected hue, expected saturation, luminosity, smoke. Brightness is computed independently of the mod:
        // V = round(base(wick) x luminosity x brightnessScale x (1 - 0.5 x smoke x (0.6 + 0.4 x (wick - 1) / 2))), base = 5/10/20
        static readonly (string code, int hue, int sat, double lum, double smoke)[] FlameExpect =
        {
            ("game:alcoholportion", 39, 3, 0.55, 0.00), ("game:oilportion-olive", 7, 6, 0.95, 0.15), ("game:oilportion-flax", 5, 7, 0.85, 0.40),
            ("game:spiritportion-apple", 9, 1, 0.50, 0.03), ("expandedfoods:lard", 4, 7, 0.75, 0.55),
            ("expandedfoods:potentspiritportion-apple", 39, 3, 0.55, 0.00), ("expandedfoods:strongspiritportion-apple", 36, 2, 0.50, 0.00),
            ("expandedfoods:foodoilportion-sunflower", 6, 6, 0.90, 0.25), ("dairyplus:ghee", 7, 5, 0.90, 0.30), ("bdorchard:oilportion-avocado", 7, 6, 0.95, 0.15),
        };

        static double EnvNum(string name, double d) => double.TryParse(Environment.GetEnvironmentVariable(name), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : d;
        static int ExpectedV(double lum, double smoke, int wick)
        {
            double eff = smoke * (0.6 + 0.4 * (wick - 1) / 2.0);
            int v = (int)Math.Round(new[] { 5, 10, 20 }[wick - 1] * lum * EnvNum("LAMPTEST_BRIGHTNESS", 1) * (1 - 0.5 * eff));
            return Math.Max(1, Math.Min(32, v));
        }

        void SetFuel(Item item)
        {
            lamp.Inventory[0].Itemstack = new ItemStack(item, 100); lamp.Inventory[0].MarkDirty();
        }

        void CheckFlames()
        {
            int good = 0, present = 0;
            foreach (var (code, hue, sat, lum, smoke) in FlameExpect)
            {
                var want = new[] { ExpectedV(lum, smoke, 1), ExpectedV(lum, smoke, 2), ExpectedV(lum, smoke, 3) };
                var item = sapi.World.GetItem(new AssetLocation(code));
                if (item == null) { log.AppendLine(string.Format("FLAME {0,-44} absent (mod not installed)", code)); continue; }
                present++;
                lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
                SetFuel(item);
                if (!lamp.Lit) Call("ToggleLightedState");
                for (int i = 0; i < 3; i++) lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Down);   // clamps at wick 1
                var got = new List<string>(); bool ok = true;
                for (int w = 1; w <= 3; w++)
                {
                    var snap = lamp.LightSnapshot;
                    bool match = snap != null && snap[0] == hue && snap[1] == sat && snap[2] == want[w - 1];
                    ok &= match;
                    got.Add(snap == null ? "null" : string.Format("({0},{1},{2}){3}", snap[0], snap[1], snap[2], match ? "" : "!=" + want[w - 1]));
                    if (w < 3) lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Up);
                }
                if (ok) good++; else flamesOk = false;
                var line = string.Format("FLAME {0,-44} hue/sat/V by wick: {1} => {2}", code, string.Join(" ", got), ok ? "PASS" : "FAIL");
                log.AppendLine(line); sapi.Logger.Notification("[LampTest] " + line);
            }
            lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
            log.AppendLine(string.Format("FLAMECHECK {0} of {1} present fuels match the expected colour and brightness", good, present));
        }

        static string DurationText(double seconds)
        {
            int minutes = (int)Math.Round(seconds / 60.0, MidpointRounding.AwayFromZero);
            if (seconds < 60) return "under a minute";
            if (minutes >= 10) minutes = (int)(Math.Round(seconds / 300.0, MidpointRounding.AwayFromZero) * 5);
            return minutes < 90 ? minutes + " min" : (minutes / 60) + " h " + (minutes % 60) + " min";
        }

        void Check(string name, bool ok, string detail)
        {
            var line = string.Format("FEATURE {0,-46} {1} {2}", name, ok ? "PASS" : "FAIL", detail);
            log.AppendLine(line); sapi.Logger.Notification("[LampTest] " + line);
            if (!ok) featuresOk = false;
        }

        void CheckFeatures()
        {
            var ba = sapi.World.BlockAccessor;
            double burnRate = EnvNum("LAMPTEST_BURNRATE", 1), brightness = EnvNum("LAMPTEST_BRIGHTNESS", 1);
            bool requireIgn = Environment.GetEnvironmentVariable("LAMPTEST_REQUIRE") != "false";
            var cfg = sapi.World.Config;
            // settings published into the world config (what clients receive)
            Check("world config burn rate", Math.Abs(cfg.GetFloat("immersivelighting_burnRate") - burnRate) < 1e-4, "= " + cfg.GetFloat("immersivelighting_burnRate"));
            Check("world config brightness", Math.Abs(cfg.GetFloat("immersivelighting_brightness") - brightness) < 1e-4, "= " + cfg.GetFloat("immersivelighting_brightness"));
            Check("world config ignition required", cfg.GetBool("immersivelighting_requireIgnition") == requireIgn, "= " + cfg.GetBool("immersivelighting_requireIgnition"));
            Check("bare-handed lighting follows the setting", lamp.CanLightBareHanded == !requireIgn, "CanLightBareHanded=" + lamp.CanLightBareHanded);

            // ignition through the game's own IIgnitable interface
            lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
            Check("no fuel: not ignitable", lamp.OnTryIgniteBlock(null, P, 0f) == Vintagestory.GameContent.EnumIgniteState.NotIgnitablePreventDefault, "");
            var av = sapi.World.GetItem(new AssetLocation("game:alcoholportion"));
            SetFuel(av);
            Check("fuel, unlit: ignitable then ignite now", lamp.OnTryIgniteBlock(null, P, 0f) == Vintagestory.GameContent.EnumIgniteState.Ignitable
                && lamp.OnTryIgniteBlock(null, P, 2f) == Vintagestory.GameContent.EnumIgniteState.IgniteNow, "");
            while (lamp.WickHeight > 1) lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Down);
            var handling = Vintagestory.API.Common.EnumHandling.PassThrough;
            lamp.OnTryIgniteBlockOver(null, P, 2f, ref handling);
            Check("igniter lights the lamp", lamp.Lit && handling == Vintagestory.API.Common.EnumHandling.PreventDefault, "lit=" + lamp.Lit);
            Check("lit: not ignitable again", lamp.OnTryIgniteBlock(null, P, 2f) == Vintagestory.GameContent.EnumIgniteState.NotIgnitable, "");

            // readout: fuel, flame, and time left (100 portions of aqua vitae, 30 s each at wick 1, scaled by the burn rate)
            var info = ((BlockLamp)ba.GetBlock(P)).GetPlacedBlockInfo(sapi.World, P, null);
            string expected1 = DurationText(100 * 30.0 / burnRate);
            Check("readout: fuel name", info.Contains(av.GetHeldItemName(new ItemStack(av, 1))) || info.Contains("Fuel:"), "");
            Check("readout: flame and smoke", info.Contains("pale blue") && info.Contains("no smoke"), "");
            Check("readout: time left at wick 1", info.Contains("Burning") && info.Contains(expected1), "expected '" + expected1 + "'");
            lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Up);
            info = ((BlockLamp)ba.GetBlock(P)).GetPlacedBlockInfo(sapi.World, P, null);
            string expected2 = DurationText(100 * 30.0 / 2 / burnRate);
            Check("readout: time left halves at wick 2", info.Contains(expected2), "expected '" + expected2 + "'");
            Call("ToggleLightedState");
            info = ((BlockLamp)ba.GetBlock(P)).GetPlacedBlockInfo(sapi.World, P, null);
            Check("readout: unlit wording", info.Contains("Would burn"), "");
            Check("handbook page text present", Vintagestory.API.Config.Lang.Get("immersivelighting:fuels-title") != "immersivelighting:fuels-title", Vintagestory.API.Config.Lang.Get("immersivelighting:fuels-title"));
            lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
        }

        void LightWorldCheck(string code, int expected)
        {
            var item = sapi.World.GetItem(new AssetLocation(code));
            if (item == null) { log.AppendLine("WORLDLIGHT skipped, " + code + " absent"); return; }
            lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
            SetFuel(item);
            if (!lamp.Lit) Call("ToggleLightedState");
            while (lamp.WickHeight > 1) lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Down);
            while (lamp.WickHeight < 3) lamp.ChangeWickHeight(BlockEntityLamp.WickMotion.Up);
        }

        void ReadWorldLight()
        {
            var line = string.Format("WORLDLIGHT olive oil at wick 3: light at lamp={0} (expected {3}), +2={1}, +4={2}", Light(P), Light(Q), Light(R), ExpectedV(0.95, 0.15, 3));
            if (Light(P) != ExpectedV(0.95, 0.15, 3)) flamesOk = false;
            log.AppendLine(line); sapi.Logger.Notification("[LampTest] " + line);
            lamp.Inventory[0].Itemstack = null; lamp.Inventory[0].MarkDirty();
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

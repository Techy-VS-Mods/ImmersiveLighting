#!/bin/bash
# Builds the lamp mod + test harness, boots an isolated headless server on a flat world, prints the light readings.
# Usage: test/run-lighttest.sh   (needs VINTAGE_STORY, defaults below)
set -e
export VINTAGE_STORY=${VINTAGE_STORY:-/mnt/linuxgame/home/user1/vintagestory}
R=$(cd "$(dirname "$0")/.." && pwd); D=${LAMPTEST_DIR:-/tmp/vs_lighttest}
(cd $R/ImmersiveLighting && dotnet build -c Release -v q 2>&1 | grep -E "error|Build succeeded" | head -5)
(cd $R/test/LampLightTest && dotnet build -c Release -v q 2>&1 | grep -E "error|Build succeeded" | head -5)
rm -rf $D && mkdir -p $D/Mods
(cd $VINTAGE_STORY && dotnet VintagestoryServer.dll --dataPath $D --genconfig >/dev/null 2>&1)
python3 - "$D" <<'PY'
import json,sys
p=sys.argv[1]+'/serverconfig.json'; c=json.load(open(p))
c['Port']=42499; c['MaxClients']=1; c['VerifyPlayerAuth']=False; c['AdvertiseServer']=False
c['WorldConfig']['WorldType']='superflat'; c['WorldConfig']['SaveFileLocation']=sys.argv[1]+'/Saves/default.vcdbs'
json.dump(c,open(p,'w'),indent=2)
PY
M=$R/ImmersiveLighting/bin/Release/Mods/mod; (cd $M && zip -q -r $D/Mods/immersivelighting_test.zip .)
T=$R/test/LampLightTest; mkdir -p $D/tt && cp $T/modinfo.json $D/tt/ && cp $T/bin/Release/LampLightTest.dll $D/tt/ && (cd $D/tt && zip -q -r $D/Mods/lamplighttest.zip .)
export LAMPTEST_OUT=$D/lamptest-results.txt
(cd $VINTAGE_STORY && timeout ${LAMPTEST_TIMEOUT:-240} dotnet VintagestoryServer.dll --dataPath $D > $D/console.txt 2>&1) || true
echo "=== RESULTS ==="; cat $LAMPTEST_OUT 2>/dev/null || { echo "NO RESULTS FILE. tail of server log:"; tail -25 $D/Logs/server-main.log; }
echo; echo "=== errors in server log ==="; grep -F "[Error]" $D/Logs/server-main.log | sed -E 's/^[0-9.]+ [0-9:.]+ //' | sort -u | head -12

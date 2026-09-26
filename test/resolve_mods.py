#!/usr/bin/env python3
"""Prints the zip files needed to load the given mod ids (seeds plus every dependency) from one or more mod folders.
Usage: resolve_mods.py <modid,modid,...> <dir> [dir...]   (prints one path per line; exits 1 if a dependency is missing)"""
import glob, json, os, re, sys, zipfile

def relaxed(t):
    t = t.lstrip("﻿"); t = re.sub(r"/\*.*?\*/", "", t, flags=re.S); t = re.sub(r"(?m)^\s*//.*$", "", t)
    t = re.sub(r",(\s*[}\]])", r"\1", t); return json.loads(t)

def modid_of(zpath):
    try:
        z = zipfile.ZipFile(zpath); n = [x for x in z.namelist() if x.lower().endswith("modinfo.json") and x.count("/") <= 1]
        d = {k.lower(): v for k, v in relaxed(z.read(n[0]).decode("utf-8", "ignore")).items()}
        return (d.get("modid") or re.sub(r"[^a-z0-9]", "", d.get("name", "").lower())), d.get("dependencies") or {}, d.get("version", "0")
    except Exception:
        return None, {}, "0"

def vt(v):
    return tuple(int(x) for x in re.findall(r"\d+", v)[:4]) or (0,)

seeds = sys.argv[1].split(","); available = {}
for d in sys.argv[2:]:
    for p in glob.glob(os.path.join(d, "*.zip")):
        mid, deps, ver = modid_of(p)
        if mid and (mid not in available or vt(ver) > vt(available[mid][2])): available[mid] = (p, deps, ver)
need, todo, missing = {}, list(seeds), []
while todo:
    m = todo.pop()
    if m in need or m in ("game", "survival", "creative"): continue
    if m not in available: missing.append(m); continue
    need[m] = available[m][0]; todo += [k for k in available[m][1] if k not in ("game", "survival", "creative")]
print("\n".join(sorted(set(need.values()))))
if missing: print("MISSING:", ",".join(missing), file=sys.stderr); sys.exit(1)

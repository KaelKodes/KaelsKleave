# Kael's Kleave

OpenMod plugin for Unturned that adds **melee cleave** — long-reach weapons can hit multiple targets in one swing.
![Kael's Kleave banner](https://i.imgur.com/2dRXnUn.png)

## Features

- Range-based cleave using the weapon's built-in `range` stat (`MinRange` config, default `2.0`)
- Extra zombie hits on zombie melee (always)
- Optional player cleave (`IncludePlayers: true` by default) — vanilla friendly-fire rules still apply
- Optional animal cleave (`IncludeAnimals: true` by default) — bears, deer, etc.
- Mixed-type cleave (`MixedMultiHit: true` by default) — one swing can hit zombies, players, and animals up to `MaxExtraTargets`
- Configurable max extra targets, damage multiplier, cleave cone angle, and repeated-melee exclusion

## Requirements

- [OpenMod](https://openmod.github.io/openmod-docs/userdoc/installation/unturned.html) on your Unturned dedicated server

## Install

1. Build: `dotnet build -c Release`
2. From `bin/Release/netstandard2.1/`, copy into your server:
   - `KaelKodes.KaelsKleave.dll` → `openmod/plugins/`
   - `KaelKodes.KaelsKleave/config.yaml` → `openmod/plugins/KaelKodes.KaelsKleave/config.yaml`
3. Restart the server (or `openmod reload` if permitted)

OR

1. Go to https://github.com/KaelKodes/KaelsKleave/releases
2. Grab the latest KaelKodes.KaelsKleave.dll
3. Drop that in your Server/OpenMod/plugins folder
4. Use OpenMod Reload or restart server
5. Grab a long melee weapon and hit stuff!

OpenMod also extracts the embedded default config on first load if the subfolder config is missing. Do **not** drop a flat `config.yaml` directly in `openmod/plugins/`.

## Config

Key settings:

| Setting | Default | Description |
|---------|---------|-------------|
| `MinRange` | `2.0` | Weapons at or above this `range` can cleave. Vanilla reference: `1.75` knives/short blades; `2.0` bat, camp axe, hockey stick, golf club, chainsaw; `2.25` fire axe, sledgehammer, katana, pitchfork, pool cue; `2.5` zweihander |
| `MaxExtraTargets` | `2` | Max additional targets hit beyond the primary hit per swing |
| `SecondaryDamageMultiplier` | `1.0` | Damage scale applied to cleave (secondary) hits. `1.0` = full damage; `0.5` = half |
| `ExcludeRepeatedMelee` | `true` | When `true`, skip continuous/repeated melee weapons (e.g. chainsaw) so they do not cleave every tick |
| `IncludePlayers` | `true` | When `true`, cleave can hit players (vanilla friendly-fire / PvP rules still apply). When `false`, players are never cleave targets |
| `IncludeAnimals` | `true` | When `true`, animals (bears, deer, etc.) can be cleave targets. When `false`, animals are never cleaved |
| `MixedMultiHit` | `true` | When `true`, one swing can hit mixed types (zombie/player/animal) up to `MaxExtraTargets`. When `false`, cleave only hits the same type as the primary target |
| `CleaveAngleDegrees` | `70.0` | Half-angle of the cleave cone in front of the attacker (degrees). Larger values = wider cleave arc |

## License

MIT

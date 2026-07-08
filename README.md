# Kael's Kleave

OpenMod plugin for Unturned that adds **melee cleave** — long-reach weapons can hit multiple targets in one swing.
![Kael's Kleave banner](https://i.imgur.com/2dRXnUn.png)
## Features

- Range-based cleave using the weapon's built-in `range` stat (`MinRange` config, default `2.0`)
- Extra zombie hits on zombie melee (always)
- Optional player cleave (`IncludePlayers: true`) — vanilla friendly-fire rules still apply
- Configurable max extra targets, damage multiplier, cleave cone angle

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
| `MinRange` | `2.0` | Weapons at or above this range can cleave |
| `MaxExtraTargets` | `2` | Max additional targets per swing |
| `IncludePlayers` | `false` | Enable player cleave (PvP) |
| `SecondaryDamageMultiplier` | `1.0` | Damage scale for cleave hits |

## License

MIT

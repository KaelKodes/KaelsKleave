using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Zombies.Events;
using SDG.Unturned;
using UnityEngine;

namespace KaelKodes.KaelsKleave;

public class CleaveListener :
    IEventListener<UnturnedZombieDamagingEvent>,
    IEventListener<UnturnedPlayerDamagingEvent>
{
    private readonly ILogger<CleaveListener> _logger;
    private readonly IOptions<KaelsKleaveConfig> _config;
    private static bool s_isApplyingCleave;

    public CleaveListener(ILogger<CleaveListener> logger, IOptions<KaelsKleaveConfig> config)
    {
        _logger = logger;
        _config = config;
    }

    public Task HandleEventAsync(object? sender, UnturnedZombieDamagingEvent @event)
    {
        return HandleZombieDamagedAsync(@event);
    }

    public Task HandleEventAsync(object? sender, UnturnedPlayerDamagingEvent @event)
    {
        return HandlePlayerDamagedAsync(@event);
    }

    private async Task HandleZombieDamagedAsync(UnturnedZombieDamagingEvent @event)
    {
        if (s_isApplyingCleave || @event.IsCancelled || @event.Instigator == null)
        {
            return;
        }

        await UniTask.SwitchToMainThread();

        if (!TryGetMeleeContext(@event.Instigator.Player, out var context))
        {
            return;
        }

        var primaryZombie = @event.Zombie.Zombie;
        if (primaryZombie == null || primaryZombie.isDead)
        {
            return;
        }

        var primaryPosition = primaryZombie.transform.position;
        var candidates = new List<(Zombie zombie, float score)>();

        foreach (var region in ZombieManager.regions)
        {
            if (region == null)
            {
                continue;
            }

            foreach (var zombie in region.zombies)
            {
                if (zombie == null || zombie.isDead || zombie == primaryZombie)
                {
                    continue;
                }

                if (!IsInCleaveCone(
                        context.AimOrigin,
                        context.AimForward,
                        zombie.transform.position,
                        primaryPosition,
                        context.WeaponRange,
                        context.Settings.CleaveAngleDegrees,
                        out var score))
                {
                    continue;
                }

                candidates.Add((zombie, score));
            }
        }

        if (candidates.Count == 0)
        {
            return;
        }

        candidates.Sort((a, b) => a.score.CompareTo(b.score));

        var damage = Mathf.Max(1f, @event.DamageAmount * context.Settings.SecondaryDamageMultiplier);
        var direction = (primaryPosition - context.AimOrigin).normalized;
        var extraTargets = Math.Min(context.Settings.MaxExtraTargets, candidates.Count);

        s_isApplyingCleave = true;
        try
        {
            for (var i = 0; i < extraTargets; i++)
            {
                var extraZombie = candidates[i].zombie;
                DamageTool.damageZombie(new DamageZombieParameters
                {
                    zombie = extraZombie,
                    damage = damage,
                    times = 1f,
                    direction = direction,
                    limb = ELimb.SPINE,
                    instigator = context.Attacker,
                    ragdollEffect = @event.RagdollEffect,
                    zombieStunOverride = @event.StunOverride
                }, out _, out _);
            }

            _logger.LogDebug(
                "Cleave hit {ExtraTargets} extra zombie(s) for {Player} with {Weapon}",
                extraTargets,
                @event.Instigator!.SteamId,
                context.MeleeAsset.itemName);
        }
        finally
        {
            s_isApplyingCleave = false;
        }
    }

    private async Task HandlePlayerDamagedAsync(UnturnedPlayerDamagingEvent @event)
    {
        if (!_config.Value.IncludePlayers || s_isApplyingCleave || @event.IsCancelled)
        {
            return;
        }

        if (@event.Cause != EDeathCause.MELEE)
        {
            return;
        }

        await UniTask.SwitchToMainThread();

        var killer = PlayerTool.getPlayer(@event.Killer);
        if (killer == null || !TryGetMeleeContext(killer, out var context))
        {
            return;
        }

        var primaryPlayer = @event.Player.Player;
        if (primaryPlayer == null || primaryPlayer.life.isDead)
        {
            return;
        }

        var primaryPosition = primaryPlayer.transform.position;
        var candidates = new List<(Player player, float score)>();

        foreach (var steamPlayer in Provider.clients)
        {
            var target = steamPlayer?.player;
            if (target == null || target == primaryPlayer || target.life.isDead)
            {
                continue;
            }

            if (!DamageTool.isPlayerAllowedToDamagePlayer(context.Attacker, target)
                && !context.MeleeAsset.bypassAllowedToDamagePlayer)
            {
                continue;
            }

            if (!IsInCleaveCone(
                    context.AimOrigin,
                    context.AimForward,
                    target.transform.position,
                    primaryPosition,
                    context.WeaponRange,
                    context.Settings.CleaveAngleDegrees,
                    out var score))
            {
                continue;
            }

            candidates.Add((target, score));
        }

        if (candidates.Count == 0)
        {
            return;
        }

        candidates.Sort((a, b) => a.score.CompareTo(b.score));

        var direction = (primaryPosition - context.AimOrigin).normalized;
        var extraTargets = Math.Min(context.Settings.MaxExtraTargets, candidates.Count);

        s_isApplyingCleave = true;
        try
        {
            for (var i = 0; i < extraTargets; i++)
            {
                var extraPlayer = candidates[i].player;
                var parameters = DamagePlayerParameters.make(
                    extraPlayer,
                    EDeathCause.MELEE,
                    direction,
                    context.MeleeAsset.playerDamageMultiplier,
                    @event.Limb);
                parameters.killer = @event.Killer;
                parameters.times = context.Settings.SecondaryDamageMultiplier;
                parameters.respectArmor = true;
                parameters.trackKill = true;
                parameters.ragdollEffect = @event.RagdollEffect;
                context.MeleeAsset.initPlayerDamageParameters(ref parameters);
                DamageTool.damagePlayer(parameters, out _);
            }

            _logger.LogDebug(
                "Cleave hit {ExtraTargets} extra player(s) for killer {Killer} with {Weapon}",
                extraTargets,
                @event.Killer,
                context.MeleeAsset.itemName);
        }
        finally
        {
            s_isApplyingCleave = false;
        }
    }

    private bool TryGetMeleeContext(Player? player, out MeleeCleaveContext context)
    {
        context = default!;
        var settings = _config.Value;

        if (player?.equipment?.asset is not ItemMeleeAsset meleeAsset)
        {
            return false;
        }

        if (player.equipment.useable is not UseableMelee)
        {
            return false;
        }

        if (settings.ExcludeRepeatedMelee && meleeAsset.isRepeated)
        {
            return false;
        }

        var weaponRange = meleeAsset.range;
        if (weaponRange < settings.MinRange)
        {
            return false;
        }

        context = new MeleeCleaveContext(
            player,
            meleeAsset,
            weaponRange,
            player.look.aim.position,
            player.look.aim.forward,
            settings);
        return true;
    }

    private static bool IsInCleaveCone(
        Vector3 aimOrigin,
        Vector3 aimForward,
        Vector3 targetPosition,
        Vector3 primaryPosition,
        float weaponRange,
        float cleaveAngleDegrees,
        out float clusterDistance)
    {
        clusterDistance = 0f;

        var toTarget = targetPosition - aimOrigin;
        if (toTarget.sqrMagnitude < 0.01f)
        {
            return false;
        }

        if (toTarget.magnitude > weaponRange + 0.5f)
        {
            return false;
        }

        if (Vector3.Angle(aimForward, toTarget.normalized) > cleaveAngleDegrees)
        {
            return false;
        }

        clusterDistance = Vector3.Distance(targetPosition, primaryPosition);
        return clusterDistance <= weaponRange + 1.0f;
    }

    private readonly struct MeleeCleaveContext
    {
        public MeleeCleaveContext(
            Player attacker,
            ItemMeleeAsset meleeAsset,
            float weaponRange,
            Vector3 aimOrigin,
            Vector3 aimForward,
            KaelsKleaveConfig settings)
        {
            Attacker = attacker;
            MeleeAsset = meleeAsset;
            WeaponRange = weaponRange;
            AimOrigin = aimOrigin;
            AimForward = aimForward;
            Settings = settings;
        }

        public Player Attacker { get; }
        public ItemMeleeAsset MeleeAsset { get; }
        public float WeaponRange { get; }
        public Vector3 AimOrigin { get; }
        public Vector3 AimForward { get; }
        public KaelsKleaveConfig Settings { get; }
    }
}
